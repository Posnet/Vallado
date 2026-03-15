% ------------------------------------------------------------------------------
%
%                           function pkeplerj4
%
%  this function propagates a satellite's position and velocity vector over
%    a given time period accounting for perturbations caused by j2, j2^2, and j4.
%    to use without drag, set ndot and nddot to 0.0 as inputs. the "xxxe1"
%    variables are a mostly common denominator form of the escobal equations
%
%  author        : david vallado                  719-573-2600    1 aug 2018
%
%  inputs          description                    range / units
%    ro          - original position vector       km
%    vo          - original velocity vector       km/sec
%    ndot        - time rate of change of n       rad/sec
%    nddot       - time accel of change of n      rad/sec2
%    dtsec       - change in time                 sec
%
%  outputs       :
%    r           - updated position vector        km
%    v           - updated velocity vector        km/sec
%
%  locals        :
%    p           - semi-paramter                  km
%    a           - semior axis                    km
%    ecc         - eccentricity
%    incl        - inclination                    rad
%    argp        - argument of periapsis          rad
%    argpdot     - change in argument of perigee  rad/sec
%    raan       - longitude of the asc node      rad
%    raandot    - change in raan                rad
%    e0          - eccentric anomaly              rad
%    e1          - eccentric anomaly              rad
%    m           - mean anomaly                   rad/sec
%    mdot        - change in mean anomaly         rad/sec
%    arglat      - argument of latitude           rad
%    arglatdot   - change in argument of latitude rad/sec
%    truelon     - true longitude of vehicle      rad
%    truelondot  - change in the true longitude   rad/sec
%    lonper     - longitude of periapsis         rad
%    lonperodot  - longitude of periapsis change  rad/sec
%    n           - mean angular motion            rad/sec
%    nuo         - true anomaly                   rad
%    j2op2       - j2 over p sqyared
%    sinv,cosv   - sine and cosine of nu
%
%  coupling:
%    rv2coe      - orbit elements from position and velocity vectors
%    coe2rv      - position and velocity vectors from orbit elements
%    newtonm     - newton rhapson to find nu and eccentric anomaly
%
%  references    :
%    vallado       2007, 687, alg 64
%
% [r,v] = pkeplerj4( ro,vo, dtsec, ndot,nddot );
% ----------------------------------------------------------------------------- }

    function [r,v] = pkeplerj4( ro,vo, dtsec, ndot,nddot );

    constmath;
    % egm-08
    re         = 6378.1363;        % km
    mu         = 398600.4415;      % km3/s2
    j2 = 0.00108262617;
    j3 = -2.53241052e-06;
    j4 = -1.6198976e-06;
    j6 = -5.40666576e-07;

    [p, a, ecc, incl, raan, argp, nu, m, arglat, truelon, lonper ] = rv2coe (ro, vo);
    %     fprintf(1,'          p km       a km      ecc      incl deg     raan deg     argp deg      nu deg      m deg      arglat   truelon    lonper\n');
    %     fprintf(1,'coes %11.4f%11.4f%13.9f%13.7f%11.5f%11.5f%11.5f%11.5f%11.5f%11.5f%11.5f\n',...
    %             p,a,ecc,incl*rad,raan*rad,argp*rad,nu*rad,m*rad, ...
    %             arglat*rad,truelon*rad,lonper*rad );
    n = sqrt(mu/(a*a*a));

    cosargp = cos(argp);
    sinargp = sin(argp);
    cosi    = cos(incl);
    sini    = sin(incl);
    cosraan = cos(raan);
    sinraan = sin(raan);
    beta2 = (1.0 - ecc^2);
    sqrtbeta = sqrt(beta2);

    % ------------- find the value of j2 perturbations -------------
    % merson approach
%     nndot = 0.75*j2*n*(re/p)^2*sqrtbeta*(2.0 - 3.0*sini^2)...
%         +3.0/512.0*j2^2*n*(re/p)^4 * sqrtbeta *(320.0*ecc*ecc - 280.0*ecc^4 + (1600.0 - 1568.0*ecc*ecc + 328*ecc^4)*sini*sini ...
%             + (-2096.0 + 1072.0*ecc*ecc + 79.0*ecc^4)*sini^4)...
%         -45.0/128.0*j4*n*ecc*ecc*(re/p)^4*sqrtbeta*(-8.0 + 40.0*sini - 35.0*sini*sini)... % should be 8.0 - 40.0*sini^2 + 35.0*sini^4
%         +35.0/2048.0*j6*n*(re/p)^6*sqrtbeta*(-128.0 + 320.0*ecc^2 + 240.0*ecc^4 + ...
%         (1344.0 - 3360.0*ecc^2 - 2520.0*ecc^4)*sini + (-1512.0 + 3780.0*ecc^2 + 2835.0*ecc^4)*sini^2 ...
%         - (-1848.0 + 4620.0*ecc^2 + 3465.0*ecc^4)*sini^4);
%     mdot = n + nndot;
    
    % from escobal pg 371 (the e terms) seems best (closest to STK)
    nbar = n*(1.5*j2*(re/p)^2*sqrtbeta*((1.0 - 1.5*sini^2)) ...
        + 3.0/128.0*j2^2*(re/p)^4 * sqrtbeta*(16.0*sqrtbeta + 25.0*beta2 - 15.0 + (30.0 - 96.0*sqrtbeta - 90.0*beta2)*cosi*cosi ...
        + (105.0 + 144.0*sqrtbeta + 25.0*beta2)*cosi^4)...
        - 45.0/128.0*j4*ecc*ecc*(re/p)^4*sqrtbeta*(3.0 - 30.0*cosi*cosi + 35.0*cosi^4) );
    mdot = n + nbar;

    % merson approach
%     raandot = -1.5*n*j2*(re/p)^2*cosi...
%         +3.0/32.0*n*j2^2*(re/p)^4*cosi*(12.0 - 4.0*ecc^2 - sini^2*(80.0 + 5.0*ecc*ecc))...
%         +15.0/32.0*n*j4*(re/p)^4*cosi*(8.0 + 12.0 * ecc*ecc - sini^2*(14.0 + 21.0*ecc*ecc)) ...
%         +105.0/1024.0*j6*n*(re/p)^6*cosi*(64.0 + 160.0*ecc^2 + 120.0*ecc^4 + ...
%         -(288.0 + 720.0*ecc^2 + 540.0*ecc^4)*sini^2 + (264.0 + 660.0*ecc^2 + 495.0*ecc^4)*sini^4);        
    raandot = -1.5*j2*(re/p)^2*mdot*cosi ...
              -1.5*j2*(re/p)^2*mdot*cosi * 1.5*j2*(re/p)^2*(1.5 + ecc^2/6.0 - 2.0*sqrtbeta -(5.0/3.0 - 5.0/24.0*ecc*ecc - 3.0*sqrtbeta)*sini*sini) ...
               -35.0/8.0*j4*(re/p)^4*n*cosi*(1.0 + 1.5*ecc*ecc)*((12.0 - 21.0*sini^2)/14.0);
    % common denominators same!
    raandote1 = -1.5*j2*(re/p)^2*mdot*cosi ...
        -9.0/96.0*j2^2*(re/p)^4*mdot*cosi*(36.0 + 4.0*ecc^2 - 48.0*sqrtbeta -(40.0 - 5.0*ecc*ecc - 72.0*sqrtbeta)*sini*sini) ...
        -35.0/112.0*j4*(re/p)^4*n*cosi*(1.0 + 1.5*ecc*ecc)*(12.0 - 21.0*sini^2);

    % merson approach
%     argpdot = 0.75*n*j2*(re/p)^2*(4.0 - 5.0*sini^2) ...
%         + 9.0/384.0*n*j2^2*(re/p)^4*(56.0*ecc*ecc + (760.0 - 36.0*ecc*ecc)*sini^2 - (890.0 + 45.0*ecc*ecc)*sini^4) ...
%         -15.0/128.0*n*j4*(re/p)^4*(64.0 + 72.0*ecc*ecc - (248.0 + 252.0*ecc*ecc)*sini*sini + (196.0 + 189.0*ecc*ecc)*sini^4) ...    
%         +105.0/2048.0*j6*n*(re/p)^6*(256.0 + 960.0*ecc^2 + 320.0*ecc^4 + ...
%         -(2048.0 + 6880.0*ecc^2 + 2160.0*ecc^4)*sini^2 + (4128.0 + 13080.0*ecc^2 + 3960.0*ecc^4)*sini^4 ...
%         -(2376.0 + 14520.0*ecc^2 + 2145.0*ecc^4)*sini^6);        
    argpdot = 1.5*j2*(re/p)^2*mdot*(2.0 - 2.5*sini^2) ...
        +9.0/4.0*j2^2*(re/p)^4*mdot*(2.0 - 2.5*sini^2) * (2.0 + ecc*ecc/2.0 - 2.0*sqrtbeta -(43.0/24.0 - ecc*ecc/48.0 - 3.0*sqrtbeta)*sini*sini) ...
        -45.0/36.0*j2^2*(re/p)^4*ecc*ecc*n*cosi^4 ...
        -35.0/8.0*j4*(re/p)^4*n*(12.0/7.0 - 93.0/14.0*sini^2 ...
        +21.0/4.0*sini^4 + ecc*ecc*(27.0/14.0 - 189.0/28.0*sini*sini + 81.0/16.0*sini^4));
    % same!
    argpdote1 = 0.75*j2*(re/p)^2*mdot*(4.0 - 5.0*sini^2) ...
        +9.0/192.0*j2^2*(re/p)^4*mdot*(2.0 - 2.5*sini^2)*(96.0 + 24.0*ecc*ecc - 96.0*sqrtbeta -(86.0 - ecc*ecc - 144.0*sqrtbeta)*sini*sini) ...
        -45.0/36.0*j2^2*(re/p)^4*ecc*ecc*n*cosi^4 ...
        -35.0/896.0*j4*(re/p)^4*n*(192.0 - 744.0*sini^2 ...
        +588*sini^4 + ecc*ecc*(216.0 - 756.0*sini*sini + 567.0*sini^4));

    raandot = raandote1;
    argpdot = argpdote1;        
    fprintf(1,'diffs   %15.11g  %15.11g  %15.11f  %15.11f \n',raandote1-raandot, argpdote1-argpdot, nddot-nbar, mdot-mdot );
    
     % can put these back in if estimates of ndot etc are known
%    a     = a - 2.0*ndot*dtsec * a / (3.0*n);
%    ecc   = ecc - 2.0*(1.0 - ecc)*ndot*dtsec / (3.0*n);
%    p     = a*(1.0 - ecc*ecc);

    % ----- update the orbital elements for each orbit type --------
    if ecc < small
        % -------------  circular equatorial  ----------------
        if ( incl < small ) || ( abs(incl-pi) < small )
            truelondot= raandot + argpdot + mdot;
            truelon   = truelon  + truelondot * dtsec;
            truelon   = rem(truelon, twopi);
            fprintf(1,'circ equat\n');
        else
            % -------------  circular inclined    --------------
            raan     = raan + raandot * dtsec;
            raan     = rem(raan, twopi);
            arglatdot= argpdot + mdot;
            arglat   = arglat + arglatdot * dtsec;
            arglat   = rem(arglat, twopi);
            fprintf(1,'circ incl\n');
        end;
    else
        % -- elliptical, parabolic, hyperbolic equatorial ---
        if ( incl < small ) || ( abs(incl-pi) < small )
            lonperdot= raandot + argpdot;
            lonper   = lonper + lonperdot * dtsec;
            lonper   = rem(lonper, twopi);
            m        = m + mdot*dtsec + ndot*dtsec*dtsec + nddot*dtsec*dtsec*dtsec;
            m        = rem(m, twopi);
            [e0, nu]= newtonm(ecc, m);
            fprintf(1,'other equat\n');
        else
            % --- elliptical, parabolic, hyperbolic inclined --
            raan = raan + raandot * dtsec;
            raan = rem(raan, twopi);
            argp = argp  + argpdot  * dtsec;
            argp = rem(argp, twopi);
            m    = m + mdot*dtsec + ndot*dtsec*dtsec + nddot*dtsec*dtsec*dtsec;
            m    = rem(m, twopi);
            [e0, nu]= newtonm(ecc, m);
        end;
    end;

    % ------------- use coe2rv to find new vectors ---------------
    [r,v] = coe2rv(p, ecc, incl, raan, argp, nu, arglat, truelon, lonper);
    r = r';
    v = v';
    %        fprintf(1,'r    %15.9f%15.9f%15.9f',r );
    %        fprintf(1,' v %15.10f%15.10f%15.10f\n',v );

