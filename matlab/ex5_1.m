    %     -----------------------------------------------------------------
    %
    %                              Ex5_1.m
    %
    %  this file demonstrates example 5-1.
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
    
    conv = pi / (180*3600);
    timezone = 0;

    year = 2006;  % need UTC that will give TDT on the 2 Apr 0 hr
    mon = 4;
    day = 1;
    hr = 23;
    minute = 58;
    second = 54.816;
    [jd, jdfrac] = jday(year, mon, day, hr, minute, second);
    fprintf(1,'jd  %11.9f \n',jd+jdfrac );
    dat = 33;
    dut1 = 0.2653628;

    [ut1, tut1, jdut1,jdut1frac, utc, tai, tt, ttt, jdtt,jdttfrac, tdb, ttdb, jdtdb,jdtdbfrac ] ...
        = convtime ( year, mon, day, hr, minute, second, timezone, dut1, dat );
    fprintf(1,'input data \n\n');
    fprintf(1,' year %5i ',year);
    fprintf(1,' mon %4i ',mon);
    fprintf(1,' day %3i ',day);
    fprintf(1,' %3i:%2i:%8.6f\n ',hr,minute,second );
    fprintf(1,' dut1 %8.6f s',dut1);
    fprintf(1,' dat %3i s',dat);

    fprintf(1,'tt  %8.6f ttt  %16.12f jdtt  %18.11f ',tt,ttt,jdtt );
    [h,m,s] = sec2hms( tt );
    fprintf(1,'hms %3i %3i %8.6f \n',h,m,s);

    [rsun,rtasc,decl] = sun ( jd+jdfrac );
    fprintf(1,'sun  rtasc %14.6f deg decl %14.6f deg\n',rtasc*rad,decl*rad );
    fprintf(1,'sun newTOD %11.9f%11.9f%11.9f au\n',rsun );
    fprintf(1,'sun newTOD %14.4f%14.4f%14.4f km\n',rsun*149597870.0 );

    rsunaa = [0.9775113 0.1911521  0.0828717]*149597870.0; % astronomical alm value into km
    fprintf(1,'rs almanac  ICRF %11.9f %11.9f %11.9f km \n',rsunaa);
    %rs aa ICRF 146233608.380930990 28595947.006026998 12397429.803279001 km 
    % from jpl de430 (icrf) ephemerides are centered on jdtdb
    %   2006  4  2          146233601.8528      28595943.3080      12397428.8371     149518194.6330  1.0010661  
    %   2006  4  2           187657.2460        286860.8848        155666.9298
    fprintf(1,'rs jplde430 ICRF 146233601.8528      28595943.3080      12397428.8371   km \n\n');

    fprintf(1,'now take the TOD sun vector (analytical) and move to MOD and TOD for comparison \n'); 
    %ttt= ( jd - 2451545.0  )/ 36525.0;
    vmod = [0 0 0]';
    amod = [0 0 0]';
    [reci,veci,aeci] = mod2eci  ( rsun',vmod,amod,ttt );
    fprintf(1,'mod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    db = reci*149597870.0-rsunaa';
    fprintf(1,'delta mod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',db, mag(db) );

    [reci,veci,aeci] = tod2eci  ( rsun',vmod,amod,ttt, ddpsi, ddeps );
    fprintf(1,'tod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    db = reci*149597870.0-rsunaa';
    fprintf(1,'delta tod - eci  %11.9f %11.9f %11.9f %11.4f km  \n\n',db, mag(db) );

    %         [reci,veci,aeci] = eci2mod ( rsun',vmod,amod,ttt );
    %         fprintf(1,'eci - mod  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    %         db = reci*149597870.0-rsunaa';
    %         fprintf(1,'delta eci - mod  %11.9f %11.9f %11.9f %11.4f km  \n',db, mag(db) );
    %
    %         [reci,veci,aeci] = eci2tod ( rsun',vmod,amod,ttt, ddpsi, ddeps );
    %         fprintf(1,'eci - tod  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    %         db = reci*149597870.0-rsunaa';
    %         fprintf(1,'delta eci - tod  %11.9f %11.9f %11.9f %11.4f km  \n',db, mag(db) );

    [hms] = hms2rad( 0,44,33.42 );
    [dms] = dms2rad( 4,47,18.3 );
    fprintf(1,'hms ast alm rtasc %11.9f decl %11.9f \n',hms*rad,dms*rad );


    % now try alamnac method
    [rsuna,rtasca,decla] = sunalmanac ( jd+jdfrac );
    fprintf(1,'\n\nsun  rtasc %14.6f deg decl %14.6f deg\n',rtasca*rad, decla*rad );
    fprintf(1,'sun ALM %11.9f%11.9f%11.9f au\n',rsuna );
    fprintf(1,'sun ALM %14.4f%14.4f%14.4f km\n',rsuna*149597870.0 );

    fprintf(1,'rs aa ICRF %11.9f %11.9f %11.9f km \n',rsunaa);

    fprintf(1,'now take the TOD sun vector (analytical) and move to MOD and TOD for comparison \n'); 
    %ttt= ( jd - 2451545.0  )/ 36525.0;
    vmod = [0 0 0]';
    amod = [0 0 0]';
    [reci,veci,aeci] = mod2eci  ( rsuna',vmod,amod,ttt );
    fprintf(1,'mod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    db = reci*149597870.0-rsunaa';
    fprintf(1,'delta mod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',db, mag(db) );

    [reci,veci,aeci] = tod2eci  ( rsuna',vmod,amod,ttt, ddpsi, ddeps );
    fprintf(1,'tod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    db = reci*149597870.0-rsunaa';
    fprintf(1,'delta tod - eci  %11.9f %11.9f %11.9f %11.4f km  \n\n',db, mag(db) );

    %         [reci,veci,aeci] = eci2mod ( rsuna',vmod,amod,ttt );
    %         fprintf(1,'eci - mod  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    %         db = reci*149597870.0-rsunaa';
    %         fprintf(1,'delta eci - mod  %11.9f %11.9f %11.9f %11.4f km  \n',db, mag(db) );
    %
    %         [reci,veci,aeci] = eci2tod ( rsuna',vmod,amod,ttt, ddpsi, ddeps );
    %         fprintf(1,'eci - tod  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    %         db = reci*149597870.0-rsunaa';
    %         fprintf(1,'delta eci - tod  %11.9f %11.9f %11.9f %11.4f km  \n',db, mag(db) );

    [hms] = hms2rad( 0,44,33.42 );
    [dms] = dms2rad( 4,47,18.3 );
    fprintf(1,'hms ast alm rtasc %11.9f decl %11.9f \n',hms*rad,dms*rad );

    fprintf(1,'==============================================================\n');
    % previous edition example
    year = 1994;
    mon = 4;
    day = 1;
    hr = 23;
    minute = 58;
    second = 59.816;
    [jd,jdfrac] = jday(year, mon, day, hr, minute, second);
    fprintf(1,'jd  %11.9f \n',jd+jdfrac );
    dat = 28;
    dut1 = -0.0226192;
    [ut1, tut1, jdut1,jdut1frac, utc, tai, tt, ttt, jdtt,jdttfrac, tdb, ttdb, jdtdb,jdtdbfrac ] ...
        = convtime ( year, mon, day, hr, minute, second, timezone, dut1, dat );
    fprintf(1,'input data \n\n');
    fprintf(1,' year %5i ',year);
    fprintf(1,' mon %4i ',mon);
    fprintf(1,' day %3i ',day);
    fprintf(1,' %3i:%2i:%8.6f\n ',hr,minute,second );
    fprintf(1,' dut1 %8.6f s',dut1);
    fprintf(1,' dat %3i s',dat);

    fprintf(1,'tt  %8.6f ttt  %16.12f jdtt  %18.11f ',tt,ttt,jdtt );
    [h,m,s] = sec2hms( tt );
    fprintf(1,'hms %3i %3i %8.6f \n',h,m,s);

    [rsun,rtasc,decl] = sun ( jd+jdfrac );
    fprintf(1,'sun  rtasc %14.6f deg decl %14.6f deg\n',rtasc*rad,decl*rad );
    fprintf(1,'sun ICRS %11.9f%11.9f%11.9f au\n',rsun );
    fprintf(1,'sun ICRS %14.4f%14.4f%14.4f km\n',rsun*149597870.0 );

    rsunaa = [0.9772766 0.1922635  0.0833613]*149597870.0; % astronomical alm value into km
    fprintf(1,'rs almanac MOD %11.9f %11.9f %11.9f km \n',rsunaa);

    fprintf(1,'now take the TOD sun vector (analytical) and move to MOD and TOD for comparison \n'); 
    %ttt= ( jd - 2451545.0  )/ 36525.0;
    vmod = [0 0 0]';
    amod = [0 0 0]';
    [reci,veci,aeci] = mod2eci  ( rsun',vmod,amod,ttt );
    fprintf(1,'mod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    db = reci*149597870.0-rsunaa';
    fprintf(1,'delta mod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',db, mag(db) );

    [reci,veci,aeci] = tod2eci  ( rsun',vmod,amod,ttt, ddpsi, ddeps );
    fprintf(1,'tod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    db = reci*149597870.0-rsunaa';
    fprintf(1,'delta tod - eci  %11.9f %11.9f %11.9f %11.4f km  \n\n',db, mag(db) );

    %         [reci,veci,aeci] = eci2mod ( rsun',vmod,amod,ttt );
    %         fprintf(1,'eci - mod  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    %         db = reci*149597870.0-rsunaa';
    %         fprintf(1,'delta eci - mod  %11.9f %11.9f %11.9f %11.4f km  \n',db, mag(db) );
    %
    %         [reci,veci,aeci] = eci2tod ( rsun',vmod,amod,ttt, ddpsi, ddeps );
    %         fprintf(1,'eci - tod  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    %         db = reci*149597870.0-rsunaa';
    %         fprintf(1,'delta eci - tod  %11.9f %11.9f %11.9f %11.4f km  \n',db, mag(db) );

    % now try alamnac method
    [rsuna,rtasca,decla] = sunalmanac ( jd+jdfrac );
    fprintf(1,'\n\nsun  rtasc %14.6f deg decl %14.6f deg\n',rtasca*rad, decla*rad );
    fprintf(1,'sun ALM %11.9f%11.9f%11.9f au\n',rsuna );
    fprintf(1,'sun ALM %14.4f%14.4f%14.4f km\n',rsuna*149597870.0 );

    fprintf(1,'rs aa ICRF %11.9f %11.9f %11.9f km \n',rsunaa);

    fprintf(1,'now take the TOD sun vector (analytical) and move to MOD and TOD for comparison \n'); 
    %ttt= ( jd - 2451545.0  )/ 36525.0;
    vmod = [0 0 0]';
    amod = [0 0 0]';
    [reci,veci,aeci] = mod2eci  ( rsuna',vmod,amod,ttt );
    fprintf(1,'mod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    db = reci*149597870.0-rsunaa';
    fprintf(1,'delta mod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',db, mag(db) );

    [reci,veci,aeci] = tod2eci  ( rsuna',vmod,amod,ttt, ddpsi, ddeps );
    fprintf(1,'tod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    db = reci*149597870.0-rsunaa';
    fprintf(1,'delta tod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',db, mag(db) );

    fprintf(1,'==============================================================\n');
    % another example tdt = 29+32.184 secs less than 4/2 at 0 hrs
    year = 1995;
    mon = 4;
    day = 1;
    hr = 23;
    minute = 58;
    second = 58.816;
    [jd,jdfrac] = jday(year, mon, day, hr, minute, second);
    fprintf(1,'jd  %11.9f \n',jd+jdfrac );
    dat = 29;
    dut1 = 0.1535663;
    [ut1, tut1, jdut1,jdut1frac, utc, tai, tt, ttt, jdtt,jdttfrac, tdb, ttdb, jdtdb,jdtdbfrac ] ...
        = convtime ( year, mon, day, hr, minute, second, timezone, dut1, dat );
    fprintf(1,'input data \n\n');
    fprintf(1,' year %5i ',year);
    fprintf(1,' mon %4i ',mon);
    fprintf(1,' day %3i ',day);
    fprintf(1,' %3i:%2i:%8.6f\n ',hr,minute,second );
    fprintf(1,' dut1 %8.6f s',dut1);
    fprintf(1,' dat %3i s',dat);

    fprintf(1,'ut1 %8.6f tut1 %16.12f jdut1 %18.11f ',ut1,tut1,jdut1 );
    [h,m,s] = sec2hms( ut1 );
    fprintf(1,'hms %3i %3i %8.6f \n',h,m,s);
    fprintf(1,'utc %8.6f ',utc );
    [h,m,s] = sec2hms( utc );
    fprintf(1,'hms %3i %3i %8.6f \n',h,m,s);
    fprintf(1,'tai %8.6f',tai );
    [h,m,s] = sec2hms( tai );
    fprintf(1,'hms %3i %3i %8.6f \n',h,m,s);
    fprintf(1,'tt  %8.6f ttt  %16.12f jdtt  %18.11f ',tt,ttt,jdtt );
    [h,m,s] = sec2hms( tt );
    fprintf(1,'hms %3i %3i %8.6f \n',h,m,s);
    fprintf(1,'tdb %8.6f ttdb %16.12f jdtdb %18.11f\n',tdb,ttdb,jdtdb );

    [rsun,rtasc,decl] = sun ( jd+jdfrac );
    fprintf(1,'sun  rtasc %14.6f deg decl %14.6f deg\n',rtasc*rad,decl*rad );
    fprintf(1,'sun ICRS %11.9f%11.9f%11.9f au\n',rsun );
    fprintf(1,'sun ICRS %14.4f%14.4f%14.4f km\n',rsun*149597870.0 );

    rsunaa = [0.9781158 0.1884327  0.0816997]*149597870.0; % astronomical alm value into km
    fprintf(1,'rs almanac MOD %11.9f %11.9f %11.9f km \n',rsunaa);

    %ttt= ( jd - 2451545.0  )/ 36525.0;
    vmod = [0 0 0]';
    amod = [0 0 0]';
    [reci,veci,aeci] = mod2eci  ( rsun',vmod,amod,ttt );
    fprintf(1,'mod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    db = reci*149597870.0-rsunaa';
    fprintf(1,'delta mod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',db, mag(db) );

    [reci,veci,aeci] = tod2eci  ( rsun',vmod,amod,ttt, ddpsi, ddeps );
    fprintf(1,'tod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    db = reci*149597870.0-rsunaa';
    fprintf(1,'delta tod - eci  %11.9f %11.9f %11.9f %11.4f km  \n\n',db, mag(db) );

    %         [reci,veci,aeci] = eci2mod ( rsun',vmod,amod,ttt );
    %         fprintf(1,'eci - mod  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    %         db = reci*149597870.0-rsunaa';
    %         fprintf(1,'delta eci - mod  %11.9f %11.9f %11.9f %11.4f km  \n',db, mag(db) );
    %
    %         [reci,veci,aeci] = eci2tod ( rsun',vmod,amod,ttt, ddpsi, ddeps );
    %         fprintf(1,'eci - tod  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    %         db = reci*149597870.0-rsunaa';
    %         fprintf(1,'delta eci - tod  %11.9f %11.9f %11.9f %11.4f km  \n',db, mag(db) );

    % now try alamnac method
    [rsuna,rtasca,decla] = sunalmanac ( jd+jdfrac );
    fprintf(1,'\n\nsun  rtasc %14.6f deg decl %14.6f deg\n',rtasca*rad, decla*rad );
    fprintf(1,'sun ALM %11.9f%11.9f%11.9f au\n',rsuna );
    fprintf(1,'sun ALM %14.4f%14.4f%14.4f km\n',rsuna*149597870.0 );

    fprintf(1,'rs aa ICRF %11.9f %11.9f %11.9f km \n',rsunaa);

    fprintf(1,'now take the TOD sun vector (analytical) and move to MOD and TOD for comparison \n'); 
    %ttt= ( jd - 2451545.0  )/ 36525.0;
    vmod = [0 0 0]';
    amod = [0 0 0]';
    [reci,veci,aeci] = mod2eci  ( rsuna',vmod,amod,ttt );
    fprintf(1,'mod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    db = reci*149597870.0-rsunaa';
    fprintf(1,'delta mod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',db, mag(db) );

    [reci,veci,aeci] = tod2eci  ( rsuna',vmod,amod,ttt, ddpsi, ddeps );
    fprintf(1,'tod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',reci*149597870.0, mag(reci)*149597870.0 );
    db = reci*149597870.0-rsunaa';
    fprintf(1,'delta tod - eci  %11.9f %11.9f %11.9f %11.4f km  \n',db, mag(db) );


