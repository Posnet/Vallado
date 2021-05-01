    %     -----------------------------------------------------------------
    %
    %                              Ex4_1.m
    %
    %  this file demonstrates example 4-1.
    %
    %                          companion code for
    %             fundamentals of astrodynamics and applications
    %                                 2013
    %                            by david vallado
    %
    %     (h)               email davallado@gmail.com
    %     (w) 719-573-2600, email dvallado@agi.com
    %
    %     *****************************************************************
    %
    %  current :
    %            16 feb 19  david vallado
    %                         update for new constants
    %  changes :
    %            13 feb 07  david vallado
    %                         original baseline
    %
    %     *****************************************************************

    constmath;

    fprintf(1,'--------------- book angle conversion tests ----------------------------\n' );
    latgd = 39.007/rad;
    lon = -104.883/rad;
    alt = 2.19456; % km

    year = 2004;
    mon  =   5;
    day  =  14;
    hr   =    12;
    min  =   0;
    sec  =   0.00;

    year = 1994;
    mon  =   5;
    day  =  14;
    hr   =    13;
    min  =   11;
    sec  =   20.59856;

    dut1 =  0.0;
    dat  = 32;
    xp   =  0.0;
    yp   =  0.0;
    lod  =  0.0;
    timezone = 0;
    order =  106;
    terms = 2;


    [ut1, tut1, jdut1, utc, tai, tt, ttt, jdtt, tdb, ttdb, jdtdb ] ...
        = convtime ( year, mon, day, hr, min, sec, timezone, dut1, dat );
    fprintf(1,'ut1 %8.6f tut1 %16.12f jdut1 %18.11f\n',ut1,tut1,jdut1 );
    fprintf(1,'utc %8.6f\n',utc );
    fprintf(1,'tai %8.6f\n',tai );
    fprintf(1,'tt  %8.6f ttt  %16.12f jdtt  %18.11f\n',tt,ttt,jdtt );
    fprintf(1,'tdb %8.6f ttdb %16.12f jdtdb %18.11f\n',tdb,ttdb,jdtdb );

    [lst, gst] = lstime(lon, jdut1);
    fprintf(1,'lst %11.7f gst %11.7f \n',lst*rad, gst*rad );

    for i = 1:2
        if i == 1
            fprintf(1,'\n-------- Neptune test baseline test \n' );
            reci = [1752246215.0; -3759563433.0; -1577568105.0];
            veci = [-18.324; 18.332; 7.777 ];
            aeci = [0.001;0.002;0.003];
            r = reci;
            v = veci;
            rr    =  29.664361*149597870.0;
            rtasc = 294.98914583/rad;
            decl  = -20.8234944/rad;
            % old book value                drr   = (149598023.0*(29.649616 - 29.664361))/86400.0
            drr   = (149597870.0*(29.649616 - 29.664361))/86400.0
            drtasc= -0.00000012244/rad;
            ddecl = -0.00000001794/rad;
            [reci,veci] = radec2rv(rr,rtasc,decl,drr,drtasc,ddecl)
        end
        if i == 2
            fprintf(1,'\n-------- closer test baseline test \n' );
            rr    =  12756.0;
            rtasc = 294.98914583/rad;
            decl  = -20.8234944/rad;
            drr   = 6.798514;
            drtasc= -0.00000012244/rad;
            ddecl = -0.00000001794/rad;
            [reci,veci] = radec2rv(rr,rtasc,decl,drr,drtasc,ddecl);
        end
        % geoc
        fprintf(1,'r    %14.7f%14.7f%14.7f',reci );
        fprintf(1,' v %14.9f%14.9f%14.9f\n',veci );

        [rr,rtasc,decl,drr,drtasc,ddecl] = rv2radec( reci,veci );
        fprintf(1,'            rho km       rtasc deg     decl deg      drho km/s      drtasc deg/s   ddecl deg/s \n' );
        if rtasc < 0.0
            rtasc = rtasc + twopi;
        end;
        fprintf(1,'radec  %14.7f %14.7f %14.7f',rr,rtasc*rad,decl*rad );
        fprintf(1,' %14.7f %14.12f %14.12f\n',drr,drtasc*rad,ddecl*rad );

        [r,v] = radec2rv(rr,rtasc,decl,drr,drtasc,ddecl);
        fprintf(1,'r    %14.7f %14.7f %14.7f',r );
        fprintf(1,' v %14.9f %14.9f %14.9f\n',v );

        % topoc
        ddpsi = 0.0;
        ddeps = 0.0;
        [trr,trtasc,tdecl,tdrr,tdrtasc,tddecl] = rv2tradc( reci,veci,latgd,lon,alt,ttt,jdut1,lod,xp,yp,terms,ddpsi,ddeps );
        fprintf(1,'           trho km      trtasc deg    tdecl deg     tdrho km/s     tdrtasc deg/s  tddecl deg/s \n' );
        if trtasc < 0.0
            trtasc = trtasc + twopi;
        end;
        fprintf(1,'tradec  %14.7f %14.7f %14.7f',trr,trtasc*rad,tdecl*rad );
        fprintf(1,' %14.7f %14.12f %14.12f\n',tdrr,tdrtasc*rad,tddecl*rad );

        %          [r,v] = tradc2rv(trr,trtasc,tdecl,tdrr,tdrtasc,tddecl,latgd,lon,alt,ttt,jdut1,lod,xp,yp,terms);
        %         fprintf(1,'r    %14.7f%14.7f%14.7f',r );
        %         fprintf(1,' v %14.9f%14.9f%14.9f\n',v );

        %horiz
        [rho,az,el,drho,daz,del] = rv2razel ( reci,veci, latgd,lon,alt,ttt,jdut1,lod,xp,yp,terms,ddpsi,ddeps );
        if az < 0.0
            az = az + twopi;
        end;
        fprintf(1,'rvraz   %14.7f %14.7f %14.7f',rho,az*rad,el*rad );
        fprintf(1,' %14.7f %14.12f %14.12f\n',drho,daz*rad,del*rad );

        [r,v] = razel2rv ( rho,az,el,drho,daz,del,latgd,lon,alt,ttt,jdut1,lod,xp,yp,terms,ddpsi,ddeps );
        fprintf(1,'r    %14.7f %14.7f %14.7f',r );
        fprintf(1,' v %14.9f %14.9f %14.9f\n',v );


        % ecl lat lon
        [rr,elon,elat,drr,delon,delat] = rv2ell( reci,veci );
        fprintf(1,'            rho km        elon deg     elat deg      drho km/s       delon deg/s   delat deg/s \n' );
        if elon < 0.0
            elon = elon + twopi;
        end;
        fprintf(1,'ell      %14.7f %14.7f %14.7f',rr,elon*rad,elat*rad );
        fprintf(1,' %14.7f %14.12f %14.12f\n',drr,delon*rad,delat*rad );

        [r,v] = ell2rv(rr,elon,elat,drr,delon,delat);
        fprintf(1,'r    %14.7f %14.7f %14.7f',r );
        fprintf(1,' v %14.9f %14.9f %14.9f\n',v );
    end % for
    

    
    
    
    
    rad    = 180.0 / pi;
    twopi = 2.0 * pi;
    % 1" to rad
    convrt = pi / (3600.0*180.0);
 
    latgd = 39.2799/rad;
    lon = -104.773/rad;
    alt = 2.01996; % km

    year = 2019;
    mon  =  10;
    day  =  11;
    hr   =    18;
    min  =   0;
    sec  =   0.00;
    [jd, jdfrac] =  jday( year, mon, day, hr, min, sec );

    dut1 =  -0.1513779;  % s
    dat  = 37;
    xp   =  0.188608* convrt;  % " to rad
    yp   =  0.299718 * convrt;
    lod  =  0.0000168* convrt;
    timezone = 0;
    ddpsi =  -0.115348* convrt;
    ddeps =  -0.009532* convrt;
    dx  = -0.000004* convrt;
    dy  = 0.000136* convrt;
    
    order =  106;
    terms = 2;

    [ut1, tut1, jdut1, utc, tai, tt, ttt, jdtt, tdb, ttdb, jdtdb ] ...
        = convtime ( year, mon, day, hr, min, sec, timezone, dut1, dat );
    fprintf(1,'ut1 %8.6f tut1 %16.12f jdut1 %18.11f\n',ut1,tut1,jdut1 );
    fprintf(1,'utc %8.6f\n',utc );
    fprintf(1,'tai %8.6f\n',tai );
    fprintf(1,'tt  %8.6f ttt  %16.12f jdtt  %18.11f\n',tt,ttt,jdtt );
    fprintf(1,'tdb %8.6f ttdb %16.12f jdtdb %18.11f\n',tdb,ttdb,jdtdb );

    [lst, gst] = lstime(lon, jd + jdfrac);
    fprintf(1,'lst %11.7f gst %11.7f \n',lst*rad, gst*rad );
  
    rho = 100000.0;
    az = 40.0/rad;
    el = 20.0/rad;

    % horizontal
    [r, v] = razel2rv ( rho,az,el,0.0, 0.0, 0.0,latgd,lon,alt, ttt,jdut1,lod,xp,yp,terms,ddpsi,ddeps );
    fprintf(1,'r    %14.7f %14.7f %14.7f',r );
    fprintf(1,' v %14.9f %14.9f %14.9f\n',v );

    [rho,az,el,drho,daz,del] = rv2razel ( r, v, latgd,lon,alt,ttt,jdut1,lod,xp,yp,terms,ddpsi,ddeps );
    if az < 0.0
        az = az + twopi;
    end;
    fprintf(1,'rvraz   %14.7f %14.7f %14.7f',rho,az*rad,el*rad );
    fprintf(1,' %14.7f %14.12f %14.12f\n',drho,daz*rad,del*rad );
    
    
    % geocentric
    [rr,rtasc,decl,drr,drtasc,ddecl] = rv2radec( r, v );
    fprintf(1,'            rho km       rtasc deg     decl deg      drho km/s      drtasc deg/s   ddecl deg/s \n' );
    if rtasc < 0.0
        rtasc = rtasc + twopi;
    end;
    fprintf(1,'radec  %14.7f %14.7f %14.7f',rr,rtasc*rad,decl*rad );
    fprintf(1,' %14.7f %14.12f %14.12f\n',drr,drtasc*rad,ddecl*rad );
    
    [r,v] = radec2rv(rr,rtasc,decl,drr,drtasc,ddecl);
    fprintf(1,'r    %14.7f %14.7f %14.7f',r );
    fprintf(1,' v %14.9f %14.9f %14.9f\n',v );
    
    % topocentric
    ddpsi = 0.0;
    ddeps = 0.0;
    [trr,trtasc,tdecl,tdrr,tdrtasc,tddecl] = rv2tradc( r, v,latgd,lon,alt,ttt,jdut1,lod,xp,yp,terms,ddpsi,ddeps );
    fprintf(1,'           trho km      trtasc deg    tdecl deg     tdrho km/s     tdrtasc deg/s  tddecl deg/s \n' );
    if trtasc < 0.0
        trtasc = trtasc + twopi;
    end;
    fprintf(1,'tradec  %14.7f %14.7f %14.7f',trr,trtasc*rad,tdecl*rad );
    fprintf(1,' %14.7f %14.12f %14.12f\n',tdrr,tdrtasc*rad,tddecl*rad );
    
    
    
    
