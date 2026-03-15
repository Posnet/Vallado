        %     -----------------------------------------------------------------
        %
        %                              Ex7_2.m
        %
        %  this file demonstrates example 7-2.
        %
        %                          companion code for
        %             fundamentals of astrodynamics and applications
        %                                 2007
        %                            by david vallado
        %
        %     (w) 719-573-2600, email dvallado@agi.com
        %
        %     *****************************************************************
        %
        %  current :
        %             9 oct 07  david vallado
        %                         original
        %  changes :
        %             9 oct 07  david vallado
        %                         original baseline
        %
        %     *****************************************************************

        constmath;
        constastro;

        %     re = 6378.137;
        %     mu = 3.986004418e5;
        tu = 86400.0;

        %    re = 6378.145;
        %    mu = 3.986005e5;

        %    re = 149597870.0;  % km in 1 au
        %    mu = 1.32712428e11;
        %    tu = 86400.0;

        %    re = 1.0;  % 1 au
        %    mu = 1.0;
        %    tu = 1.0 / 58.132440906; % days in one solar tu

        convrt = pi / (180*3600.0);

        casenum = 0;  % 0 book
      %  casenum = 4;  % test

        diffsites = 'n';

        % typerun = 'l'; % laplace
        % typerun = 'g'; % gauss
        % typerun = 'd'; % doubler
        % typerun = 'o'; % gooding
        typerun = 'a';  % all

        filedat =load(append('Sat11Ex',int2str(casenum),'.dat'));

        if casenum == 0
            r2ans = [5897.954130507     5791.046114526     6682.733686585];
            v2ans = [  -4.393910234     4.576816355     1.482423676];

            dut1 = -0.609641;
            dat   = 35;
            xp    =  0.137495 * convrt;
            yp    =  0.342416 * convrt;
            lod   =  0.0;
            timezone= 0;
            terms = 0;
            ddpsi = 0.0;
            ddeps = 0.0;
        end
        if casenum == 1
            % at 8-20-07 11:50,
            r2ans = [5897.954130507     5791.046114526     6682.733686585];
            v2ans = [  -4.393910234     4.576816355     1.482423676];

            dut1  =  -0.1639883;
            dat   = 33;
            xp    =  0.210428 * convrt;
            yp    =  0.286899 * convrt;
            lod   =  0.0;
            timezone= 0;
            terms = 0;
            ddpsi = 0.0;
            ddeps = 0.0;
        end
        if casenum == 2
            % at 8-20-12 11:48:28.000 center time,
            %             year  =  2012;
            %             mon   =   8;
            %             day   =  20;
            %             hr    =  11;
            %             min   =  55;
            %             sec   =  28.0000;
            dut1  =  -0.6096413;
            dat   = 35; % leap second in July 2012
            xp    =  0.137495 * convrt;
            yp    =  0.342416 * convrt;
            lod   =  0.0;
            timezone= 0;
            terms = 0;
            ddpsi = 0.0;
            ddeps = 0.0;
        end
        % set eop parameters for new cases
        if casenum == 3
            dut1  =  0; %-0.6096413;
            dat   = 34; % leap second in July 2012
            xp    =  0.0; %137495 * convrt;
            yp    =  0.0; %342416 * convrt;
            lod   =  0.0;
            timezone= 0;
            terms = 0;
            ddpsi = 0.0;
            ddeps = 0.0;

            obs1 = 1; %62
            obs2 = 2; %70  72
            obs3 = 3; %74 102
        end;

        if casenum == 4
            dut1  =  -0.1069721;  
            dat   = 37;  
            xp    =  0.148470 * convrt;
            yp    =  0.246564 * convrt;
            lod   =  0.0;
            timezone= 0;
            terms = 0;
            ddpsi = 0.0; %-0.113370; units
            ddeps = 0.0; %-0.007262;

            obs1 = 1; %62
            obs2 = 2; %70  72
            obs3 = 3; %74 102
            %2021 11 12 59530  0.148470  0.246564 -0.1069721  0.0001709 -0.113370 -0.007262  0.000222 -0.000052  37
        end;
        
        switch(casenum)
            case 0
                answ="# 0  ans a = 12246.023  e = 0.2000  i = 40.00  330.000          km";
            case 1
                answ="# 1  old Book      2.0  ---- COEs: a= 12756.274 e= 0.20000 i= 62.3000   40.00   20.00 Topocentric case";
            case 2
                answ="# 2  ESC pg 282   1.5 ---- COEs:  a= 8306.247 e= 0.16419 i= 32.8780  136.53  203.95 Topocentric case -2519.36 8068.22 -2664.48 -5.9319 0.07621 2.60215";
                diffsites = 'y';
            case 3
                answ="# 3  ESC pg 288  2.0 ---- COEs:  a= 12756.274 e= 0.20000 i= 62.3000   40.00   20.00 Topocentric case";
                diffsites = 'n';
            case 4
                answ="# 4  ESC pg 289   10.0 ---- COEs:  a= 63781.37 e= 0.30000 i= 45.0000   45.00   45.00 Topocentric case";
            case 5
                answ="# 5  ESC pg 290    1.33 ---- COEs:  a= 8540.963 e= 0.17900 i= 32.8800  224.87  255.20 Topocentric case";
                diffsites = 'y';
            case 6

                answ="# 6  ESC pg 291    1.44 ---- COEs:  a= 9184.517 e= 0.23167 i= 33.2810  205.11  161.79 Topocentric case";
                diffsites = 'n';
            case 7
                answ="# 7  McCuskey pg 81 2.8                                          ";
            case 8
                answ="# 8  Taff pg 232   2.67                                          ";
            case 9
                answ="# 9  Taff pg 232   2.67 ---- COEs:  a= 26627.957 e= 0.65060 i= 64.2800  121.56  318.94 Topocentric case";
            case 10
                answ="# 10 Gauss Orig - Montenbruck pg 245 Heliocentric!               ";
            case 11
                answ="# 11 Rockwell  Obj J91A01P                                       ";
            case 12
                answ="# 12 test           2.0   ---- COEs:  a= 42163.95 e= 0.00100 i=  3.3000   40.00   20.00 Topocentric case";
            case 13;
                answ="# 13 Der AAS 19-626 1:LEO 8597 a= 7358.3561 e= 0.001327600 i= 82.9703 263.5730 322.2972 79.6467 79.4971 41.9439 -14.5  -5504.8  4880.2  1.2167  4.8075  5.4409";
            case 14
                answ="# 14 Der AAS 19-626 2:MEO 28983 a= 8718.4092 e= 0.232518762 i= 105.6928 19.1724 269.5213 104.1749 77.4300 13.6962 8209  2261.3  1993.4  0.5708  -1.7319  6.4897 ";
            case 15
                answ="# 15 Der AAS 19-626 3:SS 22824 a= 7183.4835 e= 0.263394025 i= 108.9880 307.9395 272.8097 105.1106 74.7419 17.9203 3632.38  -5828.17  2088.44  -2.12272  -1.21801  7.04160";
            case 16
                answ="# 16 Der AAS 19-626 4:HEO 20959 a= 26114.3997 e= 0.228037946 i= 50.5078 246.0637 207.2528 251.0742 277.0253 98.3270 16946.69  -3286.642  20413.21  0.6302  3.6057  -1.0762";
            case 17
                answ="# 17 Der AAS 19-626 5: HEO 36970 a= 28861.7539 e= 0.750899832 i= 26.4904 16.1794 55.8578 202.3578 270.6766 258.2157 1977.27  -37010.52  -17989.04  1.6269  1.6111  0.5452";
            case 18
                answ="# 18 Der AAS 19-626 6:Moly 9880 a= 27139.7858 e= 0.703343124 i= 63.6917 110.4936 250.7611 222.8043 306.6257 113.5655 -6816.554 -14641.890  23282.890  1.89019  -0.74702  -3.05218";
            case 19
                answ="# 19 Der AAS 19-626 7:GEO 27566 a= 42162.5679 e= 0.001093761 i= 3.4358 62.2774 204.0885 80.0126 79.8892 284.1011 40903.78  -9893.48  -2450.20  0.72866  2.98740  0.04471";
            case 20
                answ="# 20 Der AAS 19-626 8:GEO 27566 a= 42162.5679 e= 0.001093761 i= 3.4358 62.2774 204.0885 80.0126 79.8892 284.1011 40903.78  -9893.48  -2450.20  0.72866  2.98740  0.04471";
            case 21
                answ="# 21 Der AAS 19-626 9:GPS 377xx  a= 7792.4746 e= 0.001021058 i= 51.9889 320.6393 92.6220 310.1230 310.2124 42.7449 6485.933  -1110.318  4164.441  -1.707715  5.583869  4.138020";
            case 22
                answ="# 22 Der AAS 19-626 10:GPS 377xx  a= 7792.4746 e= 0.001021058 i= 51.9889 320.6393 92.6220 310.1230 310.2124 42.7449 6485.933  -1110.318  4164.441  -1.707715  5.583869  4.138020";
            case 23
                answ="# 23 Curtis pg 287  a= 10000 e= 0.1 i= 30.0000  270.0000 90.000 45.010";
            otherwise
                fprintf('Invalid option\n' );
        end


        % ------ read all the data in and process
        numobs = 3; % just get # of rows
        obs1 = 1;
        obs2 = 2;
        obs3 = 3;

        %load data into x y z arrays
        yeararr = filedat(:,3);
        monarr  = filedat(:,2);
        dayarr  = filedat(:,1);
        hrarr   = filedat(:,4);
        minarr  = filedat(:,5);
        secarr  = filedat(:,6);
        latarr  = filedat(:,7)/rad;
        lonarr  = filedat(:,8)/rad;
        altarr  = filedat(:,9); % km
        rtascarr  = filedat(:,10)/rad; % rad
        declarr   = filedat(:,11)/rad; % rad

        for j = 1:numobs
            [obsrecarr(j,1).jd,obsrecarr(j,1).jdf] = jday(yeararr(j),monarr(j),dayarr(j),hrarr(j),minarr(j),secarr(j));
            obsrecarr(j,1).latgd = latarr(j);  % assumes the same sensor site
            obsrecarr(j,1).lon = lonarr(j);
            obsrecarr(j,1).alt = altarr(j);
            [ut1, tut1, jdut1,jdut1frac, utc, tai, tt, ttt, jdtt,jdttfrac, tdb, ttdb, jdtdb,jdtdbfrac ] ...
                = convtime ( yeararr(j), monarr(j), dayarr(j), hrarr(j), minarr(j), secarr(j), 0, dut1, dat );
            [obsrecarr(j,1).rs,obsrecarr(j,1).vs] = site ( latarr(j),lonarr(j),altarr(j) );
            obsrecarr(j,1).ttt = ttt;
            obsrecarr(j,1).jdut1 = jdut1;
            obsrecarr(j,1).jdut1frac = jdut1frac;
            obsrecarr(j,1).xp = xp;  % rad
            obsrecarr(j,1).yp = yp;
            obsrecarr(j,1).rtasc = rtascarr(j);
            obsrecarr(j,1).decl = declarr(j);
        end

        rtasc1 = obsrecarr(obs1,1).rtasc;
        rtasc2 = obsrecarr(obs2,1).rtasc;
        rtasc3 = obsrecarr(obs3,1).rtasc;
        decl1 = obsrecarr(obs1,1).decl;
        decl2 = obsrecarr(obs2,1).decl;
        decl3 = obsrecarr(obs3,1).decl;
        jd1 = obsrecarr(obs1,1).jd;
        jdf1 = obsrecarr(obs1,1).jdf;
        jd2 = obsrecarr(obs2,1).jd;
        jdf2 = obsrecarr(obs2,1).jdf;
        jd3 = obsrecarr(obs3,1).jd;
        jdf3 = obsrecarr(obs3,1).jdf;
        rs1 = obsrecarr(obs1,1).rs;  % ecef
        vs1 = obsrecarr(obs1,1).vs;
        rs2 = obsrecarr(obs2,1).rs;
        vs2 = obsrecarr(obs2,1).vs;
        rs3 = obsrecarr(obs3,1).rs;
        vs3 = obsrecarr(obs3,1).vs;
        rs1

        [year,mon,day,hr,min,second] = invjday(obsrecarr(obs1,1).jd, obsrecarr(obs1,1).jdf);

        utc = second;
        ut1 = utc+dut1;
        tai = utc+dat;
        tt  = tai+32.184;
        [jdut1, jdut1frac] = jday(year,mon,day,hr,min,ut1);
        [jdtt, jdttfrac]  = jday(year,mon,day,hr,min,tt);
        ttt   =  (jdtt-2451545.0)/36525.0;
        fprintf(1,'year %5i ',year);
        fprintf(1,'mon %4i ',mon);
        fprintf(1,'day %3i ',day);
        fprintf(1,'hr %3i:%2i:%8.6f\n',hr,min,second );
        fprintf(1,'dut1 %8.6f s',dut1);
        fprintf(1,' dat %3i s',dat);
        fprintf(1,' xp %8.6f "',xp);
        fprintf(1,' yp %8.6f "',yp);
        fprintf(1,' lod %8.6f s\n',lod);

        % -------------- convert each site vector from ecef to eci -----------------
        a = [0;0;0];   % dummy acceleration variable for the ecef2eci routine
        [year,mon,day,hr,min,sec] = invjday(jd1,jdf1);
        [ut1, tut1, jdut1,jdut1frac, utc, tai, tt, ttt, jdtt,jdttfrac, tdb, ttdb, jdtdb,jdtdbfrac ] ...
            = convtime ( year, mon, day, hr, min, sec, timezone, dut1, dat );
        [rsite1,vseci,aeci] = ecef2eci(rs1,vs1,a,ttt,jdut1+jdut1frac,lod,xp,yp,2,ddpsi,ddeps);

        [year,mon,day,hr,min,sec] = invjday(jd2,jdf2);
        [ut1, tut1, jdut1,jdut1frac, utc, tai, tt, ttt, jdtt,jdttfrac, tdb, ttdb, jdtdb,jdtdbfrac ] ...
            = convtime ( year, mon, day, hr, min, sec, timezone, dut1, dat );
        [rsite2,vseci,aeci] = ecef2eci(rs2,vs2,a,ttt,jdut1+jdut1frac,lod,xp,yp,2,ddpsi,ddeps);

        [year,mon,day,hr,min,sec] = invjday(jd3,jdf3);
        [ut1, tut1, jdut1,jdut1frac, utc, tai, tt, ttt, jdtt,jdttfrac, tdb, ttdb, jdtdb,jdtdbfrac ] ...
            = convtime ( year, mon, day, hr, min, sec, timezone, dut1, dat );
        [rsite3,vseci,aeci] = ecef2eci(rs3,vs3,a,ttt,jdut1+jdut1frac,lod,xp,yp,2,ddpsi,ddeps); % eci


        % ---------------------- run the angles-only routine ------------------
        if typerun == 'l' || typerun == 'a'
            [r2,v2] = anglesl( decl1,decl2,decl3,rtasc1,rtasc2, ...
                rtasc3,jd1,jdf1,jd2,jdf2,jd3,jdf3, diffsites, rsite1,rsite2,rsite3 );
            %                          rtasc3,jd1,jd2,jd3, rs1,rs2,rs3, re, mu, tu );
            processtype = 'anglesl';
        end
        % -------------- write out answer --------------
        fprintf(1,'\n\ninputs: \n\n');
        [latgc,latgd,lon,alt] = ecef2ll ( rs1 ); % need to use ecef one!!
        fprintf(1,'Site obs1 %11.7f %11.7f %11.7f km  lat %11.7f lon %11.7f alt %11.7f  \n', rsite1, latgd*rad, lon*rad, alt*1000 );
        [latgc,latgd,lon,alt] = ecef2ll ( rs2 );
        fprintf(1,'Site obs2 %11.7f %11.7f %11.7f km  lat %11.7f lon %11.7f alt %11.7f  \n', rsite2, latgd*rad, lon*rad, alt*1000 );
        [latgc,latgd,lon,alt] = ecef2ll ( rs3 );
        fprintf(1,'Site obs3 %11.7f %11.7f %11.7f km  lat %11.7f lon %11.7f alt %11.7f  \n', rsite3, latgd*rad, lon*rad, alt*1000 );
        [year,mon,day,hr,min,sec] = invjday ( jd1, jdf1 );
        fprintf(1,'obs#1 %4i %2i %2i %2i %2i %6.3f ra %11.7f de %11.7f  \n', year,mon,day,hr,min,sec, rtasc1*rad, decl1*rad );
        [year,mon,day,hr,min,sec] = invjday ( jd2, jdf2 );
        fprintf(1,'obs#2 %4i %2i %2i %2i %2i %6.3f ra %11.7f de %11.7f  \n', year,mon,day,hr,min,sec, rtasc2*rad, decl2*rad );
        [year,mon,day,hr,min,sec] = invjday ( jd3, jdf3 );
        fprintf(1,'Obs#3 %4i %2i %2i %2i %2i %6.3f ra %11.7f de %11.7f  \n', year,mon,day,hr,min,sec, rtasc3*rad, decl3*rad );

        fprintf(1,'\nsolution by %s \n\n', processtype);
        fprintf(1,'r2     %11.7f   %11.7f  %11.7f er    %11.7f  %11.7f  %11.7f km \n',r2/re, r2);
        %        fprintf(1,'r2 ans %11.7f   %11.7f  %11.7f er    %11.7f  %11.7f  %11.7f km \n',r2ans/re, r2ans);

        fprintf(1,'v2     %11.7f   %11.7f  %11.7f er/tu %11.7f  %11.7f  %11.7f km/s\n',v2/velkmps, v2);
        %        fprintf(1,'v2 ans %11.7f   %11.7f  %11.7f er/tu %11.7f  %11.7f  %11.7f km/s\n',v2ans/velkmps, v2ans);

        [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coeh (r2,v2, re, mu);
        fprintf(1,'         p km          a km         ecc       incl deg     raan deg    argp deg     nu deg      m deg  \n');
        fprintf(1,'coes %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f \n',...
            p,a,ecc,incl*rad,omega*rad,argp*rad,nu*rad,m*rad );

        fprintf(1,'%s \n', answ );

        if typerun == 'g' || typerun == 'a'
            [r2,v2] = anglesg( decl1,decl2,decl3,rtasc1,rtasc2, ...
                rtasc3,jd1,jdf1,jd2,jdf2,jd3,jdf3, rsite1,rsite2,rsite3 );
            processtype = 'anglesg';
        end
        % -------------- write out answer --------------
        fprintf(1,'\n\ninputs: \n\n');
        [latgc,latgd,lon,alt] = ecef2ll ( rs1 ); % need to use ecef one!!
        fprintf(1,'Site obs1 %11.7f %11.7f %11.7f km  lat %11.7f lon %11.7f alt %11.7f  \n', rsite1, latgd*rad, lon*rad, alt*1000 );
        [latgc,latgd,lon,alt] = ecef2ll ( rs2 );
        fprintf(1,'Site obs2 %11.7f %11.7f %11.7f km  lat %11.7f lon %11.7f alt %11.7f  \n', rsite2, latgd*rad, lon*rad, alt*1000 );
        [latgc,latgd,lon,alt] = ecef2ll ( rs3 );
        fprintf(1,'Site obs3 %11.7f %11.7f %11.7f km  lat %11.7f lon %11.7f alt %11.7f  \n', rsite3, latgd*rad, lon*rad, alt*1000 );
        [year,mon,day,hr,min,sec] = invjday ( jd1, jdf1 );
        fprintf(1,'obs#1 %4i %2i %2i %2i %2i %6.3f ra %11.7f de %11.7f  \n', year,mon,day,hr,min,sec, rtasc1*rad, decl1*rad );
        [year,mon,day,hr,min,sec] = invjday ( jd2, jdf2 );
        fprintf(1,'obs#2 %4i %2i %2i %2i %2i %6.3f ra %11.7f de %11.7f  \n', year,mon,day,hr,min,sec, rtasc2*rad, decl2*rad );
        [year,mon,day,hr,min,sec] = invjday ( jd3, jdf3 );
        fprintf(1,'Obs#3 %4i %2i %2i %2i %2i %6.3f ra %11.7f de %11.7f  \n', year,mon,day,hr,min,sec, rtasc3*rad, decl3*rad );

        fprintf(1,'\nsolution by %s \n\n', processtype);
        fprintf(1,'r2     %11.7f   %11.7f  %11.7f er    %11.7f  %11.7f  %11.7f km \n',r2/re, r2);
        %        fprintf(1,'r2 ans %11.7f   %11.7f  %11.7f er    %11.7f  %11.7f  %11.7f km \n',r2ans/re, r2ans);

        fprintf(1,'v2     %11.7f   %11.7f  %11.7f er/tu %11.7f  %11.7f  %11.7f km/s\n',v2/velkmps, v2);
        %        fprintf(1,'v2 ans %11.7f   %11.7f  %11.7f er/tu %11.7f  %11.7f  %11.7f km/s\n',v2ans/velkmps, v2ans);

        [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coeh (r2,v2, re, mu);
        fprintf(1,'         p km          a km         ecc       incl deg     raan deg    argp deg     nu deg      m deg  \n');
        fprintf(1,'coes %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f \n',...
            p,a,ecc,incl*rad,omega*rad,argp*rad,nu*rad,m*rad );

        fprintf(1,'%s \n', answ );
pause;
        if typerun == 'd' || typerun == 'a'
            [r2,v2] = anglesdr( decl1,decl2,decl3,rtasc1,rtasc2, ...
                rtasc3,jd1,jdf1,jd2,jdf2,jd3,jdf3, rsite1,rsite2,rsite3, re, mu );
            processtype = 'anglesdr';
        end
        % -------------- write out answer --------------
        fprintf(1,'\n\ninputs: \n\n');
        [latgc,latgd,lon,alt] = ecef2ll ( rs1 ); % need to use ecef one!!
        fprintf(1,'Site obs1 %11.7f %11.7f %11.7f km  lat %11.7f lon %11.7f alt %11.7f  \n', rsite1, latgd*rad, lon*rad, alt*1000 );
        [latgc,latgd,lon,alt] = ecef2ll ( rs2 );
        fprintf(1,'Site obs2 %11.7f %11.7f %11.7f km  lat %11.7f lon %11.7f alt %11.7f  \n', rsite2, latgd*rad, lon*rad, alt*1000 );
        [latgc,latgd,lon,alt] = ecef2ll ( rs3 );
        fprintf(1,'Site obs3 %11.7f %11.7f %11.7f km  lat %11.7f lon %11.7f alt %11.7f  \n', rsite3, latgd*rad, lon*rad, alt*1000 );
        [year,mon,day,hr,min,sec] = invjday ( jd1, jdf1 );
        fprintf(1,'obs#1 %4i %2i %2i %2i %2i %6.3f ra %11.7f de %11.7f  \n', year,mon,day,hr,min,sec, rtasc1*rad, decl1*rad );
        [year,mon,day,hr,min,sec] = invjday ( jd2, jdf2 );
        fprintf(1,'obs#2 %4i %2i %2i %2i %2i %6.3f ra %11.7f de %11.7f  \n', year,mon,day,hr,min,sec, rtasc2*rad, decl2*rad );
        [year,mon,day,hr,min,sec] = invjday ( jd3, jdf3 );
        fprintf(1,'Obs#3 %4i %2i %2i %2i %2i %6.3f ra %11.7f de %11.7f  \n', year,mon,day,hr,min,sec, rtasc3*rad, decl3*rad );

        fprintf(1,'\nsolution by %s \n\n', processtype);
        fprintf(1,'r2     %11.7f   %11.7f  %11.7f er    %11.7f  %11.7f  %11.7f km \n',r2/re, r2);
        %        fprintf(1,'r2 ans %11.7f   %11.7f  %11.7f er    %11.7f  %11.7f  %11.7f km \n',r2ans/re, r2ans);

        fprintf(1,'v2     %11.7f   %11.7f  %11.7f er/tu %11.7f  %11.7f  %11.7f km/s\n',v2/velkmps, v2);
        %        fprintf(1,'v2 ans %11.7f   %11.7f  %11.7f er/tu %11.7f  %11.7f  %11.7f km/s\n',v2ans/velkmps, v2ans);

        [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coeh (r2,v2, re, mu);
        fprintf(1,'         p km          a km         ecc       incl deg     raan deg    argp deg     nu deg      m deg  \n');
        fprintf(1,'coes %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f \n',...
            p,a,ecc,incl*rad,omega*rad,argp*rad,nu*rad,m*rad );

        fprintf(1,'%s \n', answ );

%         if typerun == 'o' || typerun == 'a'
%             [r2,v2] = anglesgood( decl1,decl2,decl3,rtasc1,rtasc2, ...
%                 rtasc3,jd1,jdf1,jd2,jdf2,jd3,jdf3, rsite1,rsite2,rsite3 );
%             processtype = 'anglesgood';
%         end
% 
%         % -------------- write out answer --------------
%         fprintf(1,'\n\ninputs: \n\n');
%         [latgc,latgd,lon,alt] = ecef2ll ( rs1 ); % need to use ecef one!!
%         fprintf(1,'Site obs1 %11.7f %11.7f %11.7f km  lat %11.7f lon %11.7f alt %11.7f  \n', rsite1, latgd*rad, lon*rad, alt*1000 );
%         [latgc,latgd,lon,alt] = ecef2ll ( rs2 );
%         fprintf(1,'Site obs2 %11.7f %11.7f %11.7f km  lat %11.7f lon %11.7f alt %11.7f  \n', rsite2, latgd*rad, lon*rad, alt*1000 );
%         [latgc,latgd,lon,alt] = ecef2ll ( rs3 );
%         fprintf(1,'Site obs3 %11.7f %11.7f %11.7f km  lat %11.7f lon %11.7f alt %11.7f  \n', rsite3, latgd*rad, lon*rad, alt*1000 );
%         [year,mon,day,hr,min,sec] = invjday ( jd1, jdf1 );
%         fprintf(1,'obs#1 %4i %2i %2i %2i %2i %6.3f ra %11.7f de %11.7f  \n', year,mon,day,hr,min,sec, rtasc1*rad, decl1*rad );
%         [year,mon,day,hr,min,sec] = invjday ( jd2, jdf2 );
%         fprintf(1,'obs#2 %4i %2i %2i %2i %2i %6.3f ra %11.7f de %11.7f  \n', year,mon,day,hr,min,sec, rtasc2*rad, decl2*rad );
%         [year,mon,day,hr,min,sec] = invjday ( jd3, jdf3 );
%         fprintf(1,'Obs#3 %4i %2i %2i %2i %2i %6.3f ra %11.7f de %11.7f  \n', year,mon,day,hr,min,sec, rtasc3*rad, decl3*rad );
% 
%         fprintf(1,'\nsolution by %s \n\n', processtype);
%         fprintf(1,'r2     %11.7f   %11.7f  %11.7f er    %11.7f  %11.7f  %11.7f km \n',r2/re, r2);
%         %        fprintf(1,'r2 ans %11.7f   %11.7f  %11.7f er    %11.7f  %11.7f  %11.7f km \n',r2ans/re, r2ans);
% 
%         fprintf(1,'v2     %11.7f   %11.7f  %11.7f er/tu %11.7f  %11.7f  %11.7f km/s\n',v2/velkmps, v2);
%         %        fprintf(1,'v2 ans %11.7f   %11.7f  %11.7f er/tu %11.7f  %11.7f  %11.7f km/s\n',v2ans/velkmps, v2ans);
% 
%         [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coeh (r2,v2, re, mu);
%         fprintf(1,'         p km          a km         ecc       incl deg     raan deg    argp deg     nu deg      m deg  \n');
%         fprintf(1,'coes %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f \n',...
%             p,a,ecc,incl*rad,omega*rad,argp*rad,nu*rad,m*rad );
% 
%         fprintf(1,'%s \n', answ );

        %         [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coeh (r2ans,v2ans, re, mu);
        %         fprintf(1,'         p km          a km         ecc       incl deg     raan deg    argp deg     nu deg      m deg  \n');
        %         fprintf(1,'coes %11.4f %11.4f %13.9f %13.7f %11.5f %11.5f %11.5f %11.5f \n',...
        %             p,a,ecc,incl*rad,omega*rad,argp*rad,nu*rad,m*rad );

        
rad = 180.0/pi;        
range = 7550.679305;
r =   1.0e+03 *[7.721359586705000  -6.293226594912000  -1.333676254902000];
magr=mag(r);
fprintf(1,'r    %11.7f  %11.7f  %11.7f    %11.7f \n',r, magr);
rho = 1.0e+03 *[4.648325989705000  -2.479803066912000  -5.409004552902000];
magrho = mag(rho);
fprintf(1,'rho  %11.7f  %11.7f  %11.7f    %11.7f \n',rho, magrho);
rs =  1.0e+03 *[3.073033597000000  -3.813423528000000   4.075328298000000];
magrs=mag(rs);
fprintf(1,'rs   %11.7f  %11.7f  %11.7f    %11.7f \n',rs, magrs);
los1 = [0.6156119   -0.3284185   -0.7163541];
fprintf(1,'los1  %11.7f  %11.7f  %11.7f    %11.7f \n',los1, mag(los1));
rtasc1 =    3.319220000000000e+02/rad;
rx=atan(rho(2)/rho(1)) * rad + 360;
decl1 = -45.753999999999998/rad;
dx=asin(rho(3)/mag(rho))*rad;
fprintf(1,'rtasc %11.7f  %11.7f decl %11.7f  %11.7f \n',rtasc1*rad, rx, decl1*rad, dx);
los1= [cos(decl1)*cos(rtasc1)  cos(decl1)*sin(rtasc1)  sin(decl1)];
fprintf(1,'los1 %11.7f  %11.7f  %11.7f    %11.7f \n',los1, mag(los1));
rtasc1x=atan(rho(2)/rho(1));
los1x= [cos(decl1)*cos(rtasc1x)  cos(decl1)*sin(rtasc1x)  sin(decl1)];
fprintf(1,'los1x %11.7f  %11.7f  %11.7f    %11.7f \n',los1x, mag(los1x));
urho = unit(rho);
fprintf(1,'urho  %11.7f  %11.7f  %11.7f    %11.7f \n',urho, mag(urho));

dotrsl2 = 2.0 * dot(rs,los1');
rhotem1 = 0.5 * (-dotrsl2 + sqrt(dotrsl2 * dotrsl2 - 4.0 * (magrs * magrs - magr * magr)))
r1 = rhotem1*los1 + rs;
fprintf(1,'r1 %11.7f  %11.7f  %11.7f    %11.7f \n',r1, mag(r1));
rhotem1 = 0.5 * (-dotrsl2 - sqrt(dotrsl2 * dotrsl2 - 4.0 * (magrs * magrs - magr * magr)))
r1 = rhotem1*los1 + rs;
fprintf(1,'r1 %11.7f  %11.7f  %11.7f    %11.7f \n',r1, mag(r1));

rr = unit(rho);
x1 = [0.0 los1(1) 0.0 rr(1)];
y1 = [0.0 los1(2) 0.0 rr(2)];
z1 = [0.0 los1(3) 0.0 rr(3)];
plot3(x1,y1,z1,'X')

mjd1 = 58896.0; rs1 =   124647954.923;
mjd2 = 58897.0; rs2 =   126054577.073;
mjd3 = 58898.0; rs3 =   127422565.338;
mjd4 = 58899.0; rs4 =   128751471.066;

cubicinterp(rs1, rs2, rs3, rs4, mjd1, mjd2, mjd3, mjd4, 58897.0416667)
