    % ----------------------------------------------------------------------------
    %
    %                           function iau06pna
    %
    %  this function calulates the transformation matrix that accounts for the
    %    effects of precession-nutation in the iau2000a theory.
    %
    %  author        : david vallado                  719-573-2600   16 jul 2004
    %
    %  revisions
    %    vallado     - consolidate with iau 2000                     14 feb 2005
    %
    %  inputs          description                    range / units
    %    ttt         - julian centuries of tt
    %
    %  outputs       :
    %    nut         - transformation matrix for ire-gcrf
    %    deltapsi    - change in longitude            rad
    %    l           - delaunay element               rad
    %    ll          - delaunay element               rad
    %    f           - delaunay element               rad
    %    d           - delaunay element               rad
    %    omega       - delaunay element               rad
    %    many others for planetary values             rad
    %
    %  locals        :
    %    x           - coordinate                     rad
    %    y           - coordinate                     rad
    %    s           - coordinate                     rad
    %    axs0        - real coefficients for x        rad
    %    a0xi        - integer coefficients for x
    %    ays0        - real coefficients for y        rad
    %    a0yi        - integer coefficients for y
    %    ass0        - real coefficients for s        rad
    %    a0si        - integer coefficients for s
    %    apn         - real coefficients for nutation rad
    %    apni        - integer coefficients for nutation
    %    appl        - real coefficients for planetary nutation rad
    %    appli       - integer coefficients for planetary nutation
    %    ttt2,ttt3,  - powers of ttt
    %    deltaeps    - change in obliquity            rad
    %
    %  coupling      :
    %    iau00in     - initialize the arrays
    %    fundarg     - find the fundamental arguments
    %    precess     - find the precession quantities
    %
    %  references    :
    %    vallado       2004, 212-214
    %
    % [ deltapsi, pnb, nut, l, l1, f, d, omega, ...
    %   lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate ...
    % ] = iau06pna (ttt);
    % ----------------------------------------------------------------------------

    function [ deltapsi, pnb, prec, nut, l, l1, f, d, omega, ...
        lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate ...
        ] = iau06pna (ttt);

    sethelp;
    showit = 'y';

    % " to rad
    convrt  = pi / (180.0*3600.0);
    deg2rad = pi / 180.0;

    ttt2 = ttt  * ttt;
    ttt3 = ttt2 * ttt;
    ttt4 = ttt2 * ttt2;
    ttt5 = ttt3 * ttt2;

    % obtain data for calculations from the 2000a theory
    opt = '06';  % a-all, r-reduced, e-1980 theory
    [ l, l1, f, d, omega, ...
        lonmer, lonven, lonear, lonmar, lonjup, lonsat, lonurn, lonnep, precrate ...
        ] = fundarg( ttt, opt );

    % ---- obtain data coefficients
    [axs0, a0xi, ays0, a0yi, ass0, a0si, apn, apni, appl, appli, agst, agsti] = iau06in;
    %        [axs0, a0xi, ays0, a0yi, ass0, a0si, apn, apni, ape, apei, agst, agsti] = iau06in;
    pnsum = 0.0;
    ensum = 0.0;
    for i = 678 : -1 : 1
        tempval = apni(i,1)*l + apni(i,2)*l1 + apni(i,3)*f + apni(i,4)*d + apni(i,5)*omega;
        tempval=mod(tempval,2*pi);  % rad
        %            pnsum = pnsum + (apn(i,1) + apn(i,2)*ttt) * sin(tempval) ...
        %                          + (apn(i,5) + apn(i,6)*ttt) * cos(tempval);
        %            ensum = ensum + (apn(i,3) + apn(i,4)*ttt) * cos(tempval) ...
        %                          + (apn(i,7) + apn(i,8)*ttt) * sin(tempval);
        % iers doesn't include the last few terms
        pnsum = pnsum + (apn(i,1) + apn(i,2)*ttt) * sin(tempval) ...
            + (apn(i,5) ) * cos(tempval);
        ensum = ensum + (apn(i,3) + apn(i,4)*ttt) * cos(tempval) ...
            + (apn(i,7) ) * sin(tempval);
    end;

    pplnsum = 0.0;
    eplnsum = 0.0;
    % data file is already reveresed
    for i = 1 : 687
        tempval = appli(i,1)*l + appli(i,2)*l1 + appli(i,3)*f + appli(i,4)*d + appli(i,5)*omega + ...
            appli(i,6)*lonmer  + appli(i,7)*lonven  + appli(i,8)*lonear  + appli(i,9)*lonmar + ...
            appli(i,10)*lonjup + appli(i,11)*lonsat + appli(i,12)*lonurn + appli(i,13)*lonnep + appli(i,14)*precrate;
        pplnsum = pplnsum + appl(i,1) * sin(tempval) + appl(i,2) * cos(tempval);
        eplnsum = eplnsum + appl(i,3) * sin(tempval) + appl(i,4) * cos(tempval);
    end;

    %  add planetary and luni-solar components.
    deltapsi = pnsum + pplnsum;  % rad
    deltaeps = ensum + eplnsum;
    if showit == 'y'
        fprintf(1,'dpsi %11.7f deltaeps %11.7f \n', deltapsi/convrt, deltaeps/convrt); 
    end


    % iau2006 approach - does not seem to be correct, close though
    % looks like they still use the iau2000a method and adjust
    %        pnsum = 0.0;
    %        % data file is not not reveresed
    %        for i = 1358 : -1 : 1
    %            tempval = apni(i,1)*l + apni(i,2)*l1 + apni(i,3)*f + apni(i,4)*d + apni(i,5)*omega + ...
    %                      apni(i,6)*lonmer  + apni(i,7)*lonven  + apni(i,8)*lonear  + apni(i,9)*lonmar + ...
    %                      apni(i,10)*lonjup + apni(i,11)*lonsat + apni(i,12)*lonurn + apni(i,13)*lonnep + apni(i,14)*precrate;
    %            if i > 1320
    %                pnsum = pnsum + (apn(i,1) * sin(tempval) + apn(i,2) * cos(tempval)) * ttt;  %note that sin and cos are reveresed ebtween n and e
    %            else
    %                pnsum = pnsum + apn(i,1) * sin(tempval) + apn(i,2) * cos(tempval);
    %            end;
    %          end;
    %
    %        ensum = 0.0;
    %        % data file is not reveresed
    %        for i = 1056 : -1 : 1
    %            tempval = apei(i,1)*l + apei(i,2)*l1 + apei(i,3)*f + apei(i,4)*d + apei(i,5)*omega + ...
    %                      apei(i,6)*lonmer  + apei(i,7)*lonven  + apei(i,8)*lonear  + apei(i,9)*lonmar + ...
    %                      apei(i,10)*lonjup + apei(i,11)*lonsat + apei(i,12)*lonurn + apei(i,13)*lonnep + apei(i,14)*precrate;
    %            if i > 1037
    %                ensum = ensum + (ape(i,1) * cos(tempval) + ape(i,2) * sin(tempval)) * ttt;
    %            else
    %                ensum = ensum + ape(i,1) * cos(tempval) + ape(i,2) * sin(tempval);
    %            end;
    %          end;
    %          %  add planetary and luni-solar components.
    %        deltapsi = pnsum;  % rad
    %        deltaeps = ensum;

    % iau2006 corrections to the iau2000a
    j2d = -2.7774e-6 * ttt * convrt;  % rad
    deltapsi = deltapsi + deltapsi * (0.4697e-6 + j2d);  % rad
    deltaeps = deltaeps + deltaeps * j2d;
    if showit == 'y'
        fprintf(1,'dpsi %11.7f deltaeps %11.7f \n', deltapsi/convrt, deltaeps/convrt); 
    end

    [prec,psia,wa,ea,xa] = precess ( ttt, '06' );
    if showit == 'y'
        fprintf(1,'prec iau 06 \n');
        fprintf(1,'%20.14f %20.14f %20.14f \n',prec );
    end

    oblo = 84381.406 * convrt; % " to rad or 448 - 406 for iau2006????

    % ----------------- find nutation matrix ----------------------
    % mean to true
    a1  = rot1mat(ea + deltaeps);
    a2  = rot3mat(deltapsi);
    a3  = rot1mat(-ea);

    % j2000 to date (precession)
    a4  = rot3mat(-xa);
    a5  = rot1mat(wa);
    a6  = rot3mat(psia);
    a7  = rot1mat(-oblo);

    % icrs to j2000
    a8  = rot1mat(-0.0068192*convrt);
    a9  = rot2mat(0.0417750*sin(oblo)*convrt);
    %      a9  = rot2mat(0.0166170*convrt);
    a10 = rot3mat(0.0146*convrt);

    if iauhelp =='y'
        fprintf(1,'p e %11.7f  %11.7f  \n',pnsum*180/pi,ensum*180/pi );
        fprintf(1,'p e %11.7f  %11.7f  \n',pnsum*3600*180/pi,ensum*3600*180/pi );
        fprintf(1,'p e %11.7f  %11.7f  \n',pplnsum*180/pi,eplnsum*180/pi );
        fprintf(1,'p e %11.7f  %11.7f  \n',pplnsum*3600*180/pi,eplnsum*3600*180/pi );
        fprintf(1,'dpsi %11.7f deps %11.7f  \n',deltapsi*180/pi,deltaeps*180/pi );
        fprintf(1,'dpsi %11.7f deps %11.7f  \n',deltapsi*3600*180/pi,deltaeps*3600*180/pi );
        fprintf(1,'psia %11.7f wa %11.7f ea %11.7f xa %11.7f  \n',psia*180/pi,wa*180/pi,ea*180/pi,xa*180/pi );
        fprintf(1,'psia %11.7f wa %11.7f ea %11.7f xa %11.7f  \n',psia*3600*180/pi,wa*3600*180/pi,ea*3600*180/pi,xa*3600*180/pi );
        % temp1 = a7*a6*a5*a4
        % temp2 = a3*a2*a1
        % temp3 = a10*a9*a8
    end;

    pnb = a10*a9*a8*a7*a6*a5*a4*a3*a2*a1;

       
    prec = a7*a6*a5*a4;
    if showit == 'y'
        fprintf(1,'prec iau 06a alt \n');
        fprintf(1,'%20.14f %20.14f %20.14f \n',prec );
    end
    
    nut = a3*a2*a1;
    if showit == 'y'
        fprintf(1,'nut iau 06a \n');
        fprintf(1,'%20.14f %20.14f %20.14f \n',nut );
    end

    frb = a10*a9*a8;
    if showit == 'y'
        fprintf(1,'frb iau 06a \n');
        fprintf(1,'%20.14f %20.14f %20.14f \n',frb );
    end





