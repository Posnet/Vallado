% ------------------------------------------------------------------------------
%
%                           function anglesg
%
%  this function solves the problem of orbit determination using three
%    optical sightings.  the solution function uses the gaussian technique.
%    there are lots of debug statements in here to test various options.
%
%  author        : david vallado                  719-573-2600    1 mar 2001
%
%  23 dec 2003
%   8 oct 2007
%
%  inputs          description                    range / units
%    re           - radius earth, sun, etc        km
%    mu           - grav param earth, sun etc     km3/s2
%    rtasc1       - right ascension #1                          rad
%    rtasc2       - right ascension #2                          rad
%    rtasc3       - right ascension #3                          rad
%    decl1        - declination #1                              rad
%    decl2        - declination #2                              rad
%    decl3        - declination #3                              rad
%    jd1, jdf1    - julian date of 1st sighting                 days from 4713 bc
%    jd2, jdf2    - julian date of 2nd sighting                 days from 4713 bc
%    jd3, jdf3    - julian date of 3rd sighting                 days from 4713 bc
%    rsite1       - eci site position vector                    km
%    rsite2       - eci site position vector                    km
%    rsite3       - eci site position vector                    km
%
%  outputs        :
%    r            - ijk position vector at t2     km
%    v            - ijk velocity vector at t2     km / s
%
%  locals         :
%    l1           - line of sight vector for 1st
%    l2           - line of sight vector for 2nd
%    l3           - line of sight vector for 3rd
%    tau          - taylor expansion series about
%                   tau ( t - to )
%    tausqr       - tau squared
%    t21t23       - (t2-t1) * (t2-t3)
%    t31t32       - (t3-t1) * (t3-t2)
%    i            - index
%    d            -
%    rho          - range from site to sat at t2  km
%    rhodot       -
%    dmat         -
%    rs1          - site vectors
%    rs2          -
%    rs3          -
%    earthrate    - velocity of earth rotation
%    p            -
%    q            -
%    oldr         -
%    oldv         -
%    f1           - f coefficient
%    g1           -
%    f3           -
%    g3           -
%    l2dotrs      -
%
%  coupling       :
%    mag          - magnitude of a vector
%    detrminant   - evaluate the determinant of a matrix
%    factor       - find roots of a polynomial
%    matmult      - multiply two matrices together
%    gibbs        - gibbs method of orbit determination
%    hgibbs       - herrick gibbs method of orbit determination
%    angl         - angl between two vectors
%
%  references     :
%    vallado       2007, 429-439
%
% [r2,v2] = anglesg ( decl1,decl2,decl3,rtasc1,rtasc2,rtasc3,jd1,jdf1, jd2,jdf2, jd3, jdf3,rs1,rs2,rs3, re, mu );
% ------------------------------------------------------------------------------

function [r2, v2] = anglesg ( decl1,decl2,decl3,rtasc1,rtasc2, ...
                    rtasc3,jd1,jdf1, jd2,jdf2, jd3, jdf3, rs1, rs2, rs3);
constastro;
rad = 180.0 / pi;

    % -------------------------  implementation   -------------------------
    ddpsi = 0.0;  % delta corrections not needed for this level of precision
    ddeps = 0.0;
    magr1in = 2.0*re; % initial guesses
    magr2in = 2.01*re;
    direct = 'y';  % direction of motion short way

    % ---------- set middle to 0, find decls to others -----------
    tau12 = (jd1 - jd2) * 86400.0 + (jdf1 - jdf2) * 86400.0; % days to sec
    tau13 = (jd1 - jd3) * 86400.0 + (jdf1 - jdf3) * 86400.0;
    tau32 = (jd3 - jd2) * 86400.0 + (jdf3 - jdf2) * 86400.0;
    fprintf(1,'jd123 %14.6f %14.6f %14.6f %14.6f %14.6f %14.6f  \n',jd1,jdf1, jd2,jdf2, jd3, jdf3);
    fprintf(1,'tau12 %14.6f tau13  %14.6f tau32  %14.6f \n',tau12,tau13, tau32);

    % ----------------  find line of sight unit vectors  ---------------
    los1(1)= cos(decl1)*cos(rtasc1);
    los1(2)= cos(decl1)*sin(rtasc1);
    los1(3)= sin(decl1);

    los2(1)= cos(decl2)*cos(rtasc2);
    los2(2)= cos(decl2)*sin(rtasc2);
    los2(3)= sin(decl2);

    los3(1)= cos(decl3)*cos(rtasc3);
    los3(2)= cos(decl3)*sin(rtasc3);
    los3(3)= sin(decl3);

    % topo to body fixed (ecef)
%     latgd = 40.0/rad;
%     lon   = -110.0/rad;
%     l1
%     outv = rot2(l1, -pi + latgd);
%     l1 = rot3(outv, -lon);
%     outv = rot2(l2, -pi + latgd);
%     l2 = rot3(outv, -lon);
%     outv = rot2(l3, -pi + latgd);
%     l3 = rot3(outv, -lon);
%     l1
%      
%     % take the middle trans from eecef to eci
%     tm =  [-0.830668624503591  -0.556765707115059   0.001258429966118;...
%    0.556766123167794  -0.830669298539751  -0.000023583565998;...
%    0.001058469658016   0.000681061045186   0.999999207898604];
% 
% 
% l1 = tm*l1';
% l2=tm*l2';
% l3=tm*l3';
    
    % ------------- find l matrix and determinant -----------------
    los1
    vs = [0 0 0]';
    aecef = [0 0 0]';
    %[l1eci,vs3,aeci] = ecef2eci(l1',vs,aecef,(jd1-2451545.0)/36525.0,jd1,0.0,0.0,0.0,0,ddpsi,ddeps);
    %[l2eci,vs3,aeci] = ecef2eci(l2',vs,aecef,(jd2-2451545.0)/36525.0,jd2,0.0,0.0,0.0,0,ddpsi,ddeps);
    %[l3eci,vs3,aeci] = ecef2eci(l3',vs,aecef,(jd3-2451545.0)/36525.0,jd3,0.0,0.0,0.0,0,ddpsi,ddeps);

    l1eci = los1;
    l2eci = los2;
    l3eci = los3;
    % leave these as they come since the topoc radec are already eci
    l1eci
    % --------- called lmati since it is only used for determ -----
    for i= 1 : 3
        lmat(i,1) = l1eci(i);
        lmat(i,2) = l2eci(i);
        lmat(i,3) = l3eci(i);
        rsmat(i,1)  = rs1(i);  % eci
        rsmat(i,2)  = rs2(i);
        rsmat(i,3)  = rs3(i);
    end;
    lmat
    
    fprintf(1,'rsmat eci %11.7f  %11.7f  %11.7f km \n',rsmat');

    % the order is right, but to print out, need '
    fprintf(1,'rsmat eci %11.7f  %11.7f  %11.7f \n',rsmat'/re);

    lmat
    fprintf(1,'this should be the inverse of what the code finds later\n');
    li = inv(lmat)
    
    % alt way of Curtis not seem to work ------------------
    p1 = cross(los2, los3);
    p2 = cross(los1, los3);
    p3 = cross(los1, los2);
    % both are the same
    dx = dot(los1,p1);
    %dx = dot(los3,p3);
    lcmat(1,1) = dot(rs1,p1);
    lcmat(2,1) = dot(rs2,p1);
    lcmat(3,1) = dot(rs3,p1);
    lcmat(1,2) = dot(rs1,p2);
    lcmat(2,2) = dot(rs2,p2);
    lcmat(3,2) = dot(rs3,p2);
    lcmat(1,3) = dot(rs1,p3);
    lcmat(2,3) = dot(rs2,p3);
    lcmat(3,3) = dot(rs3,p3);
    tau31= (jd3-jd1)*86400.0;
    lcmat
    aa = 1/dx *(-lcmat(1,2)*tau32/tau31 + lcmat(2,2) + lcmat(3,2)*tau12/tau31)
    bb = 1/(6.0*dx) *(lcmat(1,2)*(tau32*tau32 - tau31*tau31)*tau32/tau31  ...
                      + lcmat(3,2)*(tau31*tau31 - tau12*tau12) * tau12/tau31)
    % alt way of Curtis not seem to work ------------------
    
    
    d= det(lmat);
    d
    % ------------------ now assign the inverse -------------------
    lmati(1,1) = ( l2eci(2)*l3eci(3)-l2eci(3)*l3eci(2)) / d;
    lmati(2,1) = (-l1eci(2)*l3eci(3)+l1eci(3)*l3eci(2)) / d;
    lmati(3,1) = ( l1eci(2)*l2eci(3)-l1eci(3)*l2eci(2)) / d;
    lmati(1,2) = (-l2eci(1)*l3eci(3)+l2eci(3)*l3eci(1)) / d;
    lmati(2,2) = ( l1eci(1)*l3eci(3)-l1eci(3)*l3eci(1)) / d;
    lmati(3,2) = (-l1eci(1)*l2eci(3)+l1eci(3)*l2eci(1)) / d;
    lmati(1,3) = ( l2eci(1)*l3eci(2)-l2eci(2)*l3eci(1)) / d;
    lmati(2,3) = (-l1eci(1)*l3eci(2)+l1eci(2)*l3eci(1)) / d;
    lmati(3,3) = ( l1eci(1)*l2eci(2)-l1eci(2)*l2eci(1)) / d;
    lmati
    
    
    lir = lmati*rsmat

    % ------------ find f and g series at 1st and 3rd obs ---------
    %   speed by assuming circ sat vel for udot here ??
    %   some similartities in 1/6t3t1 ...
    % --- keep separated this time ----
    a1 =  tau32 / (tau32 - tau12);
    a1u=  (tau32*((tau32-tau12)^2 - tau32*tau32 )) / (6.0*(tau32 - tau12));
    a3 = -tau12 / (tau32 - tau12);
    a3u= -(tau12*((tau32-tau12)^2 - tau12*tau12 )) / (6.0*(tau32 - tau12));

    fprintf(1,'a1/a3 %11.7f  %11.7f  %11.7f  %11.7f \n',a1,a1u,a3,a3u );

    % --- form initial guess of r2 ----
    dl1=  lir(2,1)*a1 - lir(2,2) + lir(2,3)*a3;
    dl2=  lir(2,1)*a1u + lir(2,3)*a3u;
    dl1
    dl2

    % ------- solve eighth order poly not same as laplace ---------
    magrs2 = mag(rs2);
    l2dotrs= dot( los2,rs2 );
    fprintf(1,'magrs2 %11.7f  %11.7f  \n',magrs2,l2dotrs );

    poly( 1)=  1.0;  % r2^8th variable%%%%%%%%%%%%%%
    poly( 2)=  0.0;
    poly( 3)=  -(dl1*dl1 + 2.0*dl1*l2dotrs + magrs2^2);
    poly( 4)=  0.0;
    poly( 5)=  0.0;
    poly( 6)=  -2.0*mu*(l2dotrs*dl2 + dl1*dl2);
    poly( 7)=  0.0;
    poly( 8)=  0.0;
    poly( 9)=  -mu*mu*dl2*dl2;
    fprintf(1,'%11.7f \n',poly);
    
    rootarr = roots( poly );
    rootarr
    %fprintf(1,'rootarr %11.7f \n',rootarr);

    % ------------------ select the correct root ------------------
    bigr2= -99999990.0;
    % change from 1
    for j= 1 : 8
        if ( rootarr(j) > bigr2 ) & ( isreal(rootarr(j)) )
            bigr2= rootarr(j);
        end  % if (
    end
    bigr2
    

    % ------------ solve matrix with u2 better known --------------
    u= mu / ( bigr2*bigr2*bigr2 );

    c1= a1 + a1u*u;
    c2 = -1.0;
    c3= a3 + a3u*u;

    fprintf(1,'u %17.14f c1 %11.7f c3 %11.7f %11.7f \n',u,c1,c2,c3);

    cmat(1,1)= -c1;
    cmat(2,1)= -c2;
    cmat(3,1)= -c3;
    rhomat = lir*cmat;

    rhoold1=  rhomat(1,1)/c1;
    rhoold2=  rhomat(2,1)/c2;
    rhoold3=  rhomat(3,1)/c3;
    fprintf(1,'rhoold %11.7f %11.7f %11.7f \n',rhoold1,rhoold2,rhoold3);
    %   fprintf(1,'rhoold %11.7f %11.7f %11.7f \n',rhoold1/re,rhoold2/re,rhoold3/re);

    for i= 1 : 3
        r1(i)=  rhomat(1,1)*l1eci(i)/c1 + rs1(i);
        r2(i)=  rhomat(2,1)*l2eci(i)/c2 + rs2(i);
        r3(i)=  rhomat(3,1)*l3eci(i)/c3 + rs3(i);
    end
    fprintf(1,'r1 %11.7f %11.7f %11.7f \n',r1);
    fprintf(1,'r2 %11.7f %11.7f %11.7f \n',r2);
    fprintf(1,'r3 %11.7f %11.7f %11.7f \n',r3);

    pause;
    % -------- loop through the refining process ------------  while () for
    fprintf(1,'now refine the answer \n');
    rho2 = 999999.9;
    ll   = 0;
    while ((abs(rhoold2-rho2) > 1.0e-12) && (ll <= 0 ))  % ll <= 4   15
        ll = ll + 1;
        fprintf(1, ' iteration #%3i \n',ll );
        rho2 = rhoold2;  % reset now that inside while loop

        % ---------- now form the three position vectors ----------
        for i= 1 : 3
            r1(i)=  rhomat(1,1)*l1eci(i)/c1 + rs1(i);
            r2(i)= -rhomat(2,1)*l2eci(i)    + rs2(i);
            r3(i)=  rhomat(3,1)*l3eci(i)/c3 + rs3(i);
        end
        magr1 = mag( r1 );
        magr2 = mag( r2 );
        magr3 = mag( r3 );

        [v2,theta,theta1,copa,error] = gibbsh(r1,r2,r3, re, mu);

        rad = 180.0/pi;
        fprintf(1,'r1 %16.14f %16.14f %16.14f %11.7f %11.7f %16.14f \n',r1,theta*rad,theta1*rad, copa*rad);
        fprintf(1,'r2 %11.7f %11.7f %11.7f \n',r2);
        fprintf(1,'r3 %11.7f %11.7f %11.7f \n',r3);
        fprintf(1,'w gibbs km/s       v2 %11.7f %11.7f %11.7f \n',v2);

        % check if too close obs
        if ( (strcmp(error, '          ok') == 0) && ((abs(theta) < 1.0/rad) || (abs(theta1) < 1.0/rad)) ) % 0 is false
            [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coeh (r2,v2, re, mu);
            fprintf(1,'coes init ans %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f\n',...
                p,a,ecc,incl*rad,omega*rad,argp*rad,nu*rad,m*rad );
            % --- hgibbs to get middle vector ----
            [v2,theta,theta1,copa,error] = hgibbs(r1,r2,r3,jd1+jdf1,jd2+jdf2,jd3+jdf3);
            fprintf(1,'using hgibbs: ' );
        end

        [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coeh (r2,v2, re, mu);
        fprintf(1,'coes init ans %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f\n',...
            p,a,ecc,incl*rad,omega*rad,argp*rad,nu*rad,m*rad );
        %fprintf(1,'dr %11.7f m %11.7f m/s \n',1000*mag(r2-r2ans),1000*mag(v2-v2ans) );

        if ( ll <= 8 )  % 4
            % --- now get an improved estimate of the f and g series --
            u= mu / ( magr2*magr2*magr2 );
            rdot= dot(r2,v2)/magr2;
            udot= (-3.0*mu*rdot) / (magr2^4);

            fprintf(1,'u %17.15f rdot  %11.7f udot %11.7f \n',u,rdot,udot );
            tausqr= tau12*tau12;
            f1=  1.0 - 0.5*u*tausqr -(1.0/6.0)*udot*tausqr*tau12;
            %                       - (1.0/24.0) * u*u*tausqr*tausqr
            %                       - (1.0/30.0)*u*udot*tausqr*tausqr*tau1;
            g1= tau12 - (1.0/6.0)*u*tau12*tausqr - (1.0/12.0) * udot*tausqr*tausqr;
            %                       - (1.0/120.0)*u*u*tausqr*tausqr*tau1
            %                       - (1.0/120.0)*u*udot*tausqr*tausqr*tausqr;
            tausqr= tau32*tau32;
            f3=  1.0 - 0.5*u*tausqr -(1.0/6.0)*udot*tausqr*tau32;
            %                       - (1.0/24.0) * u*u*tausqr*tausqr
            %                       - (1.0/30.0)*u*udot*tausqr*tausqr*tau3;
            g3= tau32 - (1.0/6.0)*u*tau32*tausqr - (1.0/12.0) * udot*tausqr*tausqr;
            %                       - (1.0/120.0)*u*u*tausqr*tausqr*tau3
            %                       - (1.0/120.0)*u*udot*tausqr*tausqr*tausqr;
            fprintf(1,'f1 %11.7f g1 %11.7f f3 %11.7f g3 %11.7f \n',f1,g1,f3,g3 );
        else
            % -------- use exact method to find f and g -----------
            theta  = angl( r1,r2 );
            theta1 = angl( r2,r3 );

            f1= 1.0 - ( (magr1*(1.0 - cos(theta)) / p ) );
            g1= ( magr1*magr2*sin(-theta) ) / sqrt( p );  % - angl because backwards
            f3= 1.0 - ( (magr3*(1.0 - cos(theta1)) / p ) );
            g3= ( magr3*magr2*sin(theta1) ) / sqrt( p );

        end

        c1=  g3 / (f1*g3 - f3*g1);
        c3= -g1 / (f1*g3 - f3*g1);

        fprintf(1,' c1 %11.7f c3 %11.7f %11.7f \n',c1,c2,c3);

        % ----- solve for all three ranges via matrix equation ----
        cmat(1,1)= -c1;
        cmat(2,1)= -c2;
        cmat(3,1)= -c3;
        rhomat = lir*cmat;

        fprintf(1,'rhomat %11.7f %11.7f %11.7f \n',rhomat);
        %        fprintf(1,'rhomat %11.7f %11.7f %11.7f \n',rhomat/re);

        rhoold1=  rhomat(1,1)/c1;
        rhoold2=  rhomat(2,1)/c2;
        rhoold3=  rhomat(3,1)/c3;
        fprintf(1,'rhoold %11.7f %11.7f %11.7f \n',rhoold1,rhoold2,rhoold3);
        %   fprintf(1,'rhoold %11.7f %11.7f %11.7f \n',rhoold1/re,rhoold2/re,rhoold3/re);
        
        for i= 1 : 3
            r1(i)=  rhomat(1,1)*l1eci(i)/c1 + rs1(i);
            r2(i)=  rhomat(2,1)*l2eci(i)/c2 + rs2(i);
            r3(i)=  rhomat(3,1)*l3eci(i)/c3 + rs3(i);
        end
        fprintf(1,'r1 %11.7f %11.7f %11.7f \n',r1);
        fprintf(1,'r2 %11.7f %11.7f %11.7f \n',r2);
        fprintf(1,'r3 %11.7f %11.7f %11.7f \n',r3);

        fprintf(1,'====================next loop \n');
        % ----------------- check for convergence -----------------
        pause
        fprintf(1,'rhoold while  %16.14f %16.14f \n',rhoold2,rho2);
    end   % while the ranges are still changing

    % ---------------- find all three vectors ri ------------------
    for i= 1 : 3
        r1(i)=  rhomat(1,1)*l1eci(i)/c1 + rs1(i);
        r2(i)= -rhomat(2,1)*l2eci(i)    + rs2(i);
        r3(i)=  rhomat(3,1)*l3eci(i)/c3 + rs3(i);
    end

