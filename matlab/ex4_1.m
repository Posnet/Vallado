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

    % , tcg, jdtcg,jdtcgfrac, tcb, jdtcb,jdtcbfrac
    [ ut1, tut1, jdut1, jdut1frac, utc, tai, tt, ttt, jdtt, jdttfrac, ...
      tdb, ttdb, jdtdb, jdtdbfrac ] ...
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
            reci = reci;
            veci = veci;
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

        [reci,veci] = radec2rv(rr,rtasc,decl,drr,drtasc,ddecl);
        fprintf(1,'r    %14.7f %14.7f %14.7f',reci );
        fprintf(1,' v %14.9f %14.9f %14.9f\n',veci );

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

        [reci,veci] = razel2rv ( rho,az,el,drho,daz,del,latgd,lon,alt,ttt,jdut1,lod,xp,yp,terms,ddpsi,ddeps );
        fprintf(1,'r    %14.7f %14.7f %14.7f',reci );
        fprintf(1,' v %14.9f %14.9f %14.9f\n',veci );


        % ecl lat lon
        [rr,elon,elat,drr,delon,delat] = rv2ell( reci,veci );
        fprintf(1,'            rho km        elon deg     elat deg      drho km/s       delon deg/s   delat deg/s \n' );
        if elon < 0.0
            elon = elon + twopi;
        end;
        fprintf(1,'ell      %14.7f %14.7f %14.7f',rr,elon*rad,elat*rad );
        fprintf(1,' %14.7f %14.12f %14.12f\n',drr,delon*rad,delat*rad );

        [reci,veci] = ell2rv(rr,elon,elat,drr,delon,delat);
        fprintf(1,'r    %14.7f %14.7f %14.7f',reci );
        fprintf(1,' v %14.9f %14.9f %14.9f\n',veci );
    end % for
    

    
    
    % additional tests
    rad    = 180.0 / pi;
    twopi = 2.0 * pi;
    % 1" to rad
    convrt = pi / (3600.0*180.0);
 
    latgd = 20.7071/rad;
    lon = -156.257/rad;
    alt = 3.073;  % km
    fprintf(1,'\n\nrsecef = -5466.080829    -2404.282897    2242.177454 \n');
    
    year = 2021;
    mon  =  10;
    day  =  12;
    hr   =    4;
    min  =   10;
    sec  =   0.00;
    [jd, jdfrac] =  jday( year, mon, day, hr, min, sec );

    % 2021 10 11 59498  0.204294  0.265940 -0.1059506 -0.0002003 -0.116845 -0.008499  0.000164 -0.000071  37
    % 2021 10 12 59499  0.202929  0.265318 -0.1056985 -0.0003003 -0.116696 -0.008264  0.000206 -0.000037  37
    % 2021 10 13 59500  0.201347  0.265115 -0.1053769 -0.0003296 -0.116533 -0.008163  0.000249 -0.000003  37
    % 2021 10 14 59501  0.199531  0.264474 -0.1050561 -0.0002785 -0.116392 -0.008233  0.000253 -0.000031  37
 
    dut1 =  -0.1056985;  % s
    dat  = 37;
    xp   =  0.202929* convrt;  % " to rad
    yp   =  0.265318 * convrt;
    lod  =  -0.0003003;
    timezone = 0;
    ddpsi =  -0.116696* convrt;
    ddeps =  -0.008264* convrt;
    dx  = 0.000206* convrt;
    dy  = -0.000037* convrt;
    order =  106;
    terms = 2;
    % , tcg, jdtcg,jdtcgfrac, tcb, jdtcb,jdtcbfrac
    [ ut1, tut1, jdut1, jdut1frac, utc, tai, tt, ttt, jdtt, jdttfrac, ...
      tdb, ttdb, jdtdb, jdtdbfrac ] ...
      = convtime ( year, mon, day, hr, min, sec, timezone, dut1, dat );
    fprintf(1,'ut1 %8.6f tut1 %16.12f jdut1 %18.11f\n',ut1,tut1,jdut1 );
    fprintf(1,'utc %8.6f\n',utc );
    fprintf(1,'tai %8.6f\n',tai );
    fprintf(1,'tt  %8.6f ttt  %16.12f jdtt  %18.11f\n',tt,ttt,jdtt );
    fprintf(1,'tdb %8.6f ttdb %16.12f jdtdb %18.11f\n',tdb,ttdb,jdtdb );

    [lst, gst] = lstime(lon, jd + jdfrac);
    fprintf(1,'lst %11.7f gst %11.7f \n',lst*rad, gst*rad );
  
%     rho = 100000.0;
%     az = 40.0/rad;
%     el = 20.0/rad;
% 
%     % horizontal
%     [r, v] = razel2rv ( rho,az,el,0.0, 0.0, 0.0,latgd,lon,alt, ttt,jdut1,lod,xp,yp,terms,ddpsi,ddeps );
    reci = [2919.71566515,    -6559.47300411,      276.48177946]';
    veci = [-1.168779561,    -0.198323404,    7.352918872]';
    fprintf(1,'reci    %14.7f %14.7f %14.7f',reci );
    fprintf(1,' v %14.9f %14.9f %14.9f\n',veci );
                                       
    [rho,az,el,drho,daz,del] = rv2razel ( reci, veci, latgd,lon,alt,ttt,jdut1+jdut1frac,lod,xp,yp,terms,ddpsi,ddeps );
    if az < 0.0
        az = az + twopi;
    end
    fprintf(1,'rvraz   %14.7f %14.7f %14.7f',rho, az*rad, el*rad );
    fprintf(1,' %14.7f %14.12f %14.12f\n',drho, daz*rad, del*rad );
    fprintf(1,'STK 12 Oct 2021 04:09:07.155          159.523              5.000    2788.517174 \n');
    fprintf(1,'STK 12 Oct 2021 04:10:07.000          158.339              9.710    2393.490995 \n');
%  rtascdecl report
%             147.238               57.942    12 Oct 2021 04:09:00.000
%             144.255               53.546    12 Oct 2021 04:10:00.000

%rtascdav report
%12 Oct 2021 04:09:07.154735    2788.517174       159.522583           5.000332           326.863806           -57.466141
%12 Oct 2021 04:10:07.000000    2393.490995       158.338742           9.710252           323.927386           -52.964440
    
    % geocentric
    [rr,rtasc,decl,drr,drtasc,ddecl] = rv2radec( reci, veci );
    fprintf(1,'            rho km       rtasc deg     decl deg      drho km/s      drtasc deg/s   ddecl deg/s \n' );
    if rtasc < 0.0
        rtasc = rtasc + twopi;
    end
    fprintf(1,'radec  %14.7f %14.7f %14.7f',rr,rtasc*rad,decl*rad );
    fprintf(1,' %14.7f %14.12f %14.12f\n',drr,drtasc*rad,ddecl*rad );
    
    [reci,veci] = radec2rv(rr,rtasc,decl,drr,drtasc,ddecl);
    fprintf(1,'reci    %14.7f %14.7f %14.7f',reci );
    fprintf(1,' v %14.9f %14.9f %14.9f\n',veci );
    
    % topocentric
    ddpsi = 0.0;
    ddeps = 0.0;
    [trr,trtasc,tdecl,tdrr,tdrtasc,tddecl] = rv2tradc( reci, veci,latgd,lon,alt,ttt,jdut1+jdut1frac,lod,xp,yp,terms,ddpsi,ddeps );
    fprintf(1,'           trho km      trtasc deg    tdecl deg     tdrho km/s     tdrtasc deg/s  tddecl deg/s \n' );
    if trtasc < 0.0
        trtasc = trtasc + twopi;
    end
    fprintf(1,'tradec  %14.7f %14.7f %14.7f',trr,trtasc*rad,tdecl*rad );
    fprintf(1,' %14.7f %14.12f %14.12f\n',tdrr,tdrtasc*rad,tddecl*rad );
    fprintf(1,'STK 12 Oct 2021 04:09:07.155              326.864              -57.466 \n');
    fprintf(1,'STK 12 Oct 2021 04:10:07.000              323.927              -52.964 \n');
  

% satellite ecef 
% 12 Oct 2021 04:10:00.0000    -6166.715106346013    -3676.915653933220      282.460443869915  
%                              -0.605685283644995     1.601875853811738     7.350462613646496
% satellite eci vector
% 12 Oct 2021 04:10:00.0000     2919.71566515    -6559.47300411      276.48177946    
%                               -1.168779561    -0.198323404     7.352918872

    
    
    
