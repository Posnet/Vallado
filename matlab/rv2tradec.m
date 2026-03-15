% ------------------------------------------------------------------------------
%
%                           function rv2tradec
%
%  this function converts geocentric equatorial (eci) position and velocity
%    vectors into range, topcentric right acension, declination, and rates.  
%    notice the value of small as it can affect the rate term calculations. 
%    the solution uses the velocity vector to find the singular cases. also,
%    the right acension and declination rate terms are not observable unless 
%    the acceleration vector is available.
%
%  author        : david vallado           davallado@gmail.com    19 jul 2004
%
%  inputs          description                              range / units
%    reci        - eci position vector                      km
%    veci        - eci velocity vector                      km/s
%    latgd       - geodetic latitude                        -pi/2 to pi/2 rad
%    lon         - longitude of site                        -2pi to 2pi rad
%    alt         - altitude                                 km
%    ttt         - julian centuries of tt                   centuries
%    jdut1       - julian date of ut1                       days from 4713 bc
%    lod         - excess length of day                     sec
%    xp          - polar motion coefficient                 rad
%    yp          - polar motion coefficient                 rad
%    terms       - number of terms for ast calculation      0,2
%
%  outputs       :
%    rho         - satellite range from site                km
%    trtasc      - topocentric right ascension              0.0 to 2pi rad
%    tdecl       - topocentric declination                  -pi/2 to pi/2 rad
%    drho        - range rate                               km/s
%    dtrtasc     - topocentric rtasc rate                   rad / s
%    dtdecl      - topocentric decl rate                    rad / s
%
%  locals        :
%    rhoveci     - eci range vector from site               km
%    drhoveci    - eci velocity vector from site            km / s
%
%  coupling      :
%    mag         - magnitude of a vector
%    rot3        - rotation about the 3rd axis
%    rot2        - rotation about the 2nd axis
%
%  references    :
%    vallado       2022, 257, alg 26
%
%  [rho, trtasc, tdecl, drho, dtrtasc, dtdecl] = rv2tradec ( reci, veci, latgd, lon, alt, ttt,jdut1,lod,xp,yp,terms,ddpsi,ddeps );
% ------------------------------------------------------------------------------

function [rho, trtasc, tdecl, drho, dtrtasc, dtdecl] = rv2tradec ( reci, veci, latgd, lon, alt, ttt,jdut1,lod,xp,yp,terms,ddpsi,ddeps );

    constmath;

    % --------------------- implementation ------------------------
    % ----------------- get site vector in ecef -------------------
    [rsecef, vsecef] = site ( latgd, lon, alt );

    %rs
    %vs
    % -------------------- convert ecef to eci --------------------
    a = [0;0;0];
    [rseci, vseci, aeci] = ecef2eci(rsecef, vsecef, a, ttt, jdut1, lod, xp, yp, 2, ddpsi, ddeps);
    %rseci
    %vseci

    %rseci = rs;
    %vseci = vs;
    %[recef,vecef,aecef] = eci2ecef(reci,veci,aeci,ttt,jdut1,lod,xp,yp,2,0,0);
    %reci = recef;
    %veci = vecef;

    % ------- find eci slant range vector from site to satellite ---------
    rhoveci  = reci - rseci;
    drhoveci = veci - vseci;
    rho      = mag(rhoveci);

    % --------------- calculate topocentric rtasc and decl ---------------
    temp = sqrt( rhoveci(1) * rhoveci(1) + rhoveci(2) * rhoveci(2) );
    if (temp < small)
        trtasc = atan2( drhoveci(2), drhoveci(1) );
    else
        trtasc = atan2( rhoveci(2), rhoveci(1) );
    end

    % directly over the north pole
    if (temp < small)            
        tdecl = sign(rhoveci(3)) * halfpi;   % +- 90 deg
    else
        magrhoeci = mag(rhoveci);
        tdecl = asin( rhoveci(3) / magrhoeci );
    end
    if (trtasc < 0.0)
        trtasc = trtasc + 2.0*pi;
    end

    % ---------- calculate topcentric rtasc and decl rates -------------
    temp1 = -rhoveci(2) * rhoveci(2) - rhoveci(1) * rhoveci(1);
    drho = dot(rhoveci, drhoveci) / rho;
    if ( abs( temp1 ) > small )
        dtrtasc = ( drhoveci(1)*rhoveci(2) - drhoveci(2) * rhoveci(1) ) / temp1;
    else
        dtrtasc = 0.0;
    end

    if ( abs( temp ) > small )
        dtdecl = ( drhoveci(3) - drho * sin( tdecl ) ) / temp;
    else
        dtdecl = 0.0;
    end

    end   % rv2tradec
