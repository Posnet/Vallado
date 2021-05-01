    %     -----------------------------------------------------------------
    %
    %                              Ex3_13.m
    %
    %  this file demonstrates example 3-13.
    %
    %                          companion code for
    %             fundamentals of astrodyanmics and applications
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

    % -------- 1995 June 8, 20:18:3.703691
    jd = 2449877.3458762;

    fprintf(1,'jd %8.7f  \n\n', jd );

    jdfrac = jd-floor(jd);
    jd = floor(jd);

    fprintf(1,'integer %6i  \n', jd );
    fprintf(1,'fractional  %3.7f  \n\n', jdfrac);

    [year,mon,day,hr,min,sec] = invjday ( jd, jdfrac );

    fprintf(1,'year %6i  \n',year);
    fprintf(1,'mon  %3i  \n',mon);
    fprintf(1,'day  %3i  \n',day);
    fprintf(1,'hr   %3i  \n',hr);
    fprintf(1,'min  %3i  \n',min);
    fprintf(1,'sec  %3.6f  \n',sec);

    pause;
    
    

    % some stressing cases
    for i = 1:11
        if (i == 1)
            jd = 2457884.5;
            jdF = 0.160911640046296;
        end
        if (i == 2)
            jd = 2457884.5;
            jdF = -0.160911640046296;
        end
        if (i == 3)
            jd = 2457884.5;
            jdF = 0.660911640046296;
        end
        if (i == 4)
            jd = 2457884.5;
            jdF = -0.660911640046296;
        end
        if (i == 5)
            jd = 2457884.5;
            jdF = 2.160911640046296;
        end
        if (i == 6)
            jd = 2457884.660911640046296;
            jdF = 0.0;
        end
        if (i == 7)
            jd = 2457884.0;
            jdF = 2.160911640046296;
        end
        if (i == 8)
            jd = 2457884.5;
            jdF = 0.0;
        end
        if (i == 9)
            jd = 2457884.5;
            jdF = 0.5;
        end
        if (i == 10)
            jd = 2457884.5;
            jdF = 1.0;
        end
        if (i == 11)
            jd = 2457884.3;
            jdF = 1.0;
        end

        jdb = floor(jd + jdF) + 0.5;
        mfme = (jd + jdF - jdb) * 1440.0;
        if (mfme < 0.0)
            mfme = 1440.0 + mfme;
        end
        [year,mon,day,hr,min,sec] = invjday ( jd, jdF );
        if (abs(jdF) >= 1.0)
            jd = jd + floor(jdF);
            jdF = jdF - floor(jdF);
        end
        dt = jd - floor(jd) - 0.5;
        if (abs(dt) > 0.00000001)
            jd = jd - dt;
            jdF = jdF + dt;
        end
        % this gets it even to the day
        if (jdF < 0.0)
            jd = jd - 1.0;
            jdF = 1.0 + jdF;
        end
        fprintf(1,'%2i %4i %2i %2i %2i:%2i:%7.4f   %9.4f %9.4f %12.4f %8.5f %5.4f \n',...
            i, year, mon, day, hr, min, sec, mfme, hr*60.0+min+sec/60.0, jd, jdF, dt);
    end  % through stressing cases

    pause;

    [jdo, jdfraco] = jday(2017, 8, 23, 12, 15, 16);

    [jdo, jdfraco] = jday(2017, 12, 31, 12, 15, 16);
    fprintf(1,'%11.6f  %11.6f \n',jdo,jdfraco);
    [year,mon,day,hr,min,sec] = invjday ( jdo + jdfraco, 0.0 );
    fprintf(1,'%4i  %3i  %3i  %2i:%2i:%6.4f  \n', year,mon,day,hr,min,sec);
    [year,mon,day,hr,min,sec] = invjday ( jdo, jdfraco );
    fprintf(1,'%4i  %3i  %3i  %2i:%2i:%6.4f  \n', year,mon,day,hr,min,sec);

    for i = -50:50
        jd = jdo + i/10.0;
        jdfrac = jdfraco;
        [year,mon,day,hr,min,sec] = invjday ( jd + jdfrac, 0.0 );
        fprintf(1,'%4i  %3i  %3i  %2i:%2i:%6.4f  \n', year,mon,day,hr,min,sec);
        [year,mon,day,hr,min,sec] = invjday ( jd, jdfrac );
        dt = jd - floor(jd);
        fprintf(1,'%4i  %3i  %3i  %2i:%2i:%6.4f  \n\n', year,mon,day,hr,min,sec);
    end

    fprintf(1,'end first half \n');

    for i = -50:50
        jd = jdo;
        jdfrac = jdfraco + i/10.0;
        [year,mon,day,hr,min,sec] = invjday ( jd + jdfrac, 0.0 );
        fprintf(1,'%4i  %3i  %3i  %2i:%2i:%6.4f  \n', year,mon,day,hr,min,sec);
        [year,mon,day,hr,min,sec] = invjday ( jd, jdfrac );
        fprintf(1,'%4i  %3i  %3i  %2i:%2i:%6.4f  \n\n', year,mon,day,hr,min,sec);
    end




