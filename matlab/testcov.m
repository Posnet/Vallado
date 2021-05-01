        %
        %     -----------------------------------------------------------------
        %
        %                               testcov.m
        %
        %  this file tests the accuracy of the covariance functions.
        %
        %                          companion code for
        %             fundamentals of astrodyanmics and applications
        %                                2013
        %                          by david vallado
        %
        %     (h)               email davallado@gmail.com
        %     (w) 719-573-2600, email dvallado@agi.com
        %
        %     *****************************************************************
        %
        %  current :
        %            22 jun 15  david vallado
        %                         many fixes for new paper
        %  changes :
        %             9 aug 03  david vallado
        %                         fix units on n in equinoc?
        %            25 jul 03  david vallado
        %                         fixes for tm
        %            20 jul 03  david vallado
        %                         fixes to add extras for paper
        %            25 jun 03  david vallado
        %                         update for alternate cases
        %            26 may 03  david vallado
        %                         fix units and values
        %            28 oct 02  david vallado
        %                         fix covariance transformations
        %             5 sep 02  david vallado
        %                         fix cov trace input, misc
        %            26 aug 02  david vallado
        %                         work on partials for covariance and test cases
        %            17 aug 02  david vallado
        %                         work on partials for covariance
        %            30 jun 02  david vallado
        %                         breakout reduction, time, 2body
        %            24 may 02  david vallado
        %                         original baseline
        %
        %     *****************************************************************

        constastro;

        small = 1.0e-18;
        rad   = 180.0/pi;
        rad2  = rad*rad;

        anom = 'truea';
        anom = 'truen';
        %anom = 'meana';
        testnum = 3;

        ddpsi = 0.0;
        ddeps = 0.0;

        % --------------- 0. AF test
        if testnum==0
            reci = [3961.74426025;  6010.21561092; 4619.36257583 ];
            veci = [-5.314643385545; 3.964357584990; 1.752939152769 ];
            aeci = [0.001;0.002;0.003];

            year = 2000;
            mon  =   6;
            day  =  28;
            hr   =  15;
            min  =   8;
            sec  =  51.655;

            dut1 =  0.16236;
            dat  = 21;
            xp   =  0.0987;
            yp   =  0.2860;
            lod  =  0.0;
            timezone = 0;
            order = 4;
            terms = 2;
        end;

        % ------- navy test
        if testnum == 1
            reci = [-605.79221660; -5870.22951108; 3493.05319896;];
            veci = [-1.56825429; -3.70234891; -6.47948395; ];
            aeci = [0.001;0.002;0.003];
            year = 2000;
            mon  =  12;
            day  =  15;
            hr   =  16;
            min  =  58;
            sec  =  50.208;
            dut1 =  0.10597;
            dat  = 32;
            xp   =  0.0;
            yp   =  0.0;
            lod  =  0.0;
            terms = 2;
            timezone= 0;
            order = 106;
            cartcov = [ ...
                8.04204e-13,-7.41923e-13, 2.02623e-12,-3.78793e-16, 1.77302e-13,-2.48352e-13; ...
                -7.41923e-13, 2.19044e-12, 3.82034e-12,-1.19010e-15, 2.83844e-13,-3.67925e-13; ...
                2.02623e-12, 3.82034e-12, 1.97570e-11,-9.35438e-15,-1.47386e-12, 3.01542e-12; ...
                -3.78793e-16,-1.19010e-15,-9.35438e-15, 1.56236e-17, 8.86093e-17, 4.56974e-16; ...
                1.77302e-13, 2.83844e-13,-1.47386e-12, 8.86093e-17, 9.67797e-13, 7.23072e-13; ...
                -2.48352e-13,-3.67925e-13, 3.01542e-12, 4.56974e-16, 7.23072e-13, 1.84123e-12];
            %  3.06012e-12  1.16922e-11  6.72154e-11  1.28647e-13  4.11455e-13 -3.89727e-12  2.20508e-05
        end

        % ------- accuracy sensitivity test
        if testnum == 2
            reci = [-605.79221660; -5870.22951108; 3493.05319896;];
            veci = [-1.56825429; -3.70234891; -6.47948395; ];
            aeci = [0.001;0.002;0.003];

            year = 2000;
            mon  =  12;
            day  =  15;
            hr   =  16;
            min  =  58;
            sec  =  50.208;
            dut1 =  0.10597;
            dat  = 32;
            xp   =  0.0;
            yp   =  0.0;
            lod  =  0.0;
            timezone= 0;
            terms = 2;
            order = 106;
            covntw = [ ...
                1.0, 0.0, 0.0, 0.0, 0.0, 0.0; ...
                0.0, 10.0, 0.0, 0.0, 0.0, 0.0; ...
                0.0, 0.0, 1.0, 0.0, 0.0, 0.0; ...
                0.0, 0.0, 0.0, 1.0e-6, 0.0, 0.0; ...
                0.0, 0.0, 0.0, 0.0, 1.0e-4, 0.0; ...
                0.0, 0.0, 0.0, 0.0, 0.0, 1.0e-6];
            [ut1, tut1, jdut1, jdut1frac, utc, tai, tt, ttt, jdtt, jdttfrac, tdb, ttdb, jdtdb, jdtdbfrac, tcg, jdtcg,jdtcgfrac, tcb, jdtcb,jdtcbfrac  ] ...
                = convtime ( year, mon, day, hr, min, sec, timezone, dut1, dat );

            [cartstate,classstate,flstate,eqstate] = setcov(reci,veci, ...
                year,mon,day,hr,min,sec,dut1,dat,ttt,jdut1,lod,xp,yp,terms,'y',anom,ddpsi,ddeps);

            printcov( covntw,'ct','m',anom );

            [cartcov,tm] = covo22ct( covntw,cartstate );
            printcov( cartcov,'ct','m',anom );
            pause;
        end

        % ------- trace test
        if testnum == 3
            reci = [-605.79221660; -5870.22951108; 3493.05319896;];
            veci = [-1.56825429; -3.70234891; -6.47948395; ];
            aeci = [0.001;0.002;0.003];

            year = 2000;
            mon  =  12;
            day  =  15;
            hr   =  16;
            min  =  58;
            sec  =  50.208;
            dut1 =  0.10597;
            dat  = 32;
            xp   =  0.0;
            yp   =  0.0;
            lod  =  0.0;
            timezone= 0;
            terms = 2;
            order = 106;
            cartcov = [ ...
                0.81,   1.0e-2, 1.0e-2, 1.0e-3, 1.0e-3, 1.0e-3; ...
                1.0e-2, 0.81,   1.0e-2, 1.0e-3, 1.0e-3, 1.0e-3; ...
                1.0e-2, 1.0e-2, 0.81,   1.0e-3, 1.0e-3, 1.0e-3; ...
                1.0e-4, 1.0e-4, 1.0e-4, 0.000001, 1.0e-6, 1.0e-6; ...
                1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6, 0.000001, 1.0e-6; ...
                1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6, 1.0e-6, 0.000001];
            cartcov = [ ...
                1.0, 0.0, 0.0, 0.0, 0.0, 0.0; ...
                0.0, 1.0, 0.0, 0.0, 0.0, 0.0; ...
                0.0, 0.0, 1.0, 0.0, 0.0, 0.0; ...
                0.0, 0.0, 0.0, 0.000001, 0.0, 0.0; ...
                0.0, 0.0, 0.0, 0.0, 0.000001, 0.0; ...
                0.0, 0.0, 0.0, 0.0, 0.0, 0.000001];

            classcovtrace = [ ...
                0.6489403e+01,   0.2979136e-06,  0.2117582e-21,  0.5293956e-21,  0.3465237e-01,  0.9048777e-02; ...
                0.2979136e-06,   0.4353986e-13,  0.6708473e-26,  0.1091152e-25,  0.1059842e-08,  0.2770890e-09; ...
                0.2117582e-21,   0.6708473e-26,  0.5652526e-10,  0.6541195e-13,  0.8709806e-14, -0.4430849e-22; ...
                0.5293956e-21,   0.1091152e-25,  0.6541195e-13,  0.5749008e-10,  0.7654984e-11, -0.7072071e-22; ...
                0.3465237e-01,   0.1059842e-08,  0.8709806e-14,  0.7654984e-11,  0.2232401e-03,  0.5830381e-04; ...
                0.9048777e-02,   0.2770890e-09, -0.4430849e-22, -0.7072071e-22,  0.5830381e-04,  0.1522727e-04];

            eqcovtrace = [ ...
                0.5703960e-13,   0.2562410e-13,  0.2218139e-11,  0.6530893e-14, -0.1258488e-17,  0.6777803e-17; ...
                0.2562410e-13,   0.6348393e-13, -0.1958979e-11,  0.7398763e-14, -0.4223513e-17,  0.2274645e-16; ...
                0.2218139e-11,  -0.1958979e-11,  0.3562517e-09,  0.2727333e-15,  0.2373188e-12, -0.1278121e-11; ...
                0.6530893e-14,   0.7398763e-14,  0.2727333e-15,  0.1256922e-14,  0.4930381e-31, -0.1972152e-30; ...
                -0.1258488e-17,  -0.4223513e-17,  0.2373188e-12,  0.4930381e-31,  0.2292326e-13, -0.2061165e-16; ...
                0.6777803e-17,   0.2274645e-16, -0.1278121e-11, -0.1972152e-30, -0.2061165e-16,  0.2288388e-13];

            printcov( cartcov,'ct','m',anom );
            fprintf(1,'Check the xxxx (mat*transpose) of cartcov \n' );
            fprintf(1,'%16e%16e%16e%16e%16e%16e\n',(cartcov*cartcov')' );
            %            printcov( classcovtrace,'cl','t',anom );
            %            classcovt = covunits( classcovtrace,anom,'cl','m' );
            %            printcov( classcovt,'cl','m',anom );
            %            printcov( eqcovtrace,'eq','t',anom );
            %            eqcovt = covunits( eqcovtrace,anom,'eq','m' );
            %            printcov( eqcovt,'eq','m',anom );
        end

        if testnum == 4
            reci = [4364.51524926;    4748.17602940;    2430.20427647];
            veci = [   5.87962414;      -4.10294944;      -2.53527819];

            year = 2000;
            mon  =  12;
            day  =  14;
            hr   =   5;
            min  =  25;
            sec  =   3.461;
            dut1 =  0.10597;
            dat  = 32;
            xp   =  0.0;
            yp   =  0.0;
            lod  =  0.0;
            terms = 2;
            timezone= 0;
            order = 106;

            % ----- in from usstratcom is lower diagonal!!!
            eqcov = [ ...
                4.68914e-11,  1.60090e-11,  1.64731e-10, -4.38141e-16, -1.41195e-10,  1.09999e-11, -8.49933e-12; ...
                1.60090e-11,  1.06881e-11,  6.39732e-11,  6.53124e-16, -5.60554e-11,  2.56099e-11, -4.44240e-13; ...
                1.64731e-10,  6.39732e-11,  6.71336e-10,  1.04455e-14, -6.37507e-10,  9.40554e-11,  7.34847e-12; ...
                -4.38141e-16,  6.53124e-16,  1.04455e-14,  2.10897e-17,  1.59272e-14,  1.03081e-14,  1.80744e-13; ...
                -1.41195e-10, -5.60554e-11, -6.37507e-10,  1.59272e-14,  7.59844e-10, -1.05437e-10,  1.77857e-10; ...
                1.09999e-11,  2.56099e-11,  9.40554e-11,  1.03081e-14, -1.05437e-10,  1.86472e-10,  3.99631e-11; ...
                -8.49933e-12, -4.44240e-13,  7.34847e-12,  1.80744e-13,  1.77857e-10,  3.99631e-11,  4.67207e-06;];

            printcov( eqcovtrace,'eq','t',anom );
            eqcovt = covunits( eqcovtrace,anom,'eq','m' );
            printcov( eqcovt,'eq','m',anom );
        end % if testnum

        if testnum == 5
            reci = [10127.26750234;    6972.89492052;    4902.05501566];
            veci = [   -3.38546947;       0.81341143;      -5.70012872];

            year = 2000;
            mon  =  12;
            day  =   1;
            hr   =   4;
            min  =  39;
            sec  =   8.735;
            dut1 =  0.11974;
            dat  = 32;
            xp   =  0.0;
            yp   =  0.0;
            lod  =  0.0;
            terms = 2;
            timezone= 0;
            order = 106;

            eqcovtrace = [ ...
                1.79533e-09,  9.35041e-10, -1.99413e-09, -1.63427e-13,  1.05541e-10, -5.77137e-10,  5.44105e-10; ...
                9.35041e-10,  1.04209e-09, -5.91847e-10, -9.78886e-14, -4.02330e-11, -4.39526e-10,  6.10449e-10; ...
                -1.99413e-09, -5.91847e-10,  3.63062e-09,  2.62623e-13, -3.98481e-10,  9.56574e-10,  2.86557e-09; ...
                -1.63427e-13, -9.78886e-14,  2.62623e-13,  5.56451e-16,  5.18072e-14, -2.60304e-14, -2.27216e-12; ...
                1.05541e-10, -4.02330e-11, -3.98481e-10,  5.18072e-14,  1.00164e-09,  2.40555e-10, -1.10189e-10; ...
                -5.77137e-10, -4.39526e-10,  9.56574e-10, -2.60304e-14,  2.40555e-10,  2.05855e-09, -1.97222e-11; ...
                5.44105e-10,  6.10449e-10,  2.86557e-09, -2.27216e-12, -1.10189e-10, -1.97222e-11,  1.13395e-05;];

            printcov( eqcovtrace,'eq','t',anom );
            eqcovt = covunits( eqcovtrace,anom,'eq','m' );
            printcov( eqcovt,'eq','m',anom );
        end % if testnum


        %         a = 6860.7631;
        %         ecc = 0.0010640;
        %         p = a*(1.0 - ecc^2);
        %         incl = 90.0/rad; %97.65184/rad;
        %         omega = 79.54701/rad;
        %         argp = 83.86041/rad;
        %         nu = 65.21303/rad;
        %         m = 65.10238/rad;
        %         arglat = 0.0;
        %         truelon = 0.0;
        %         lonper = 0.0;
        %         [reci,veci] = coe2rv ( p,ecc,incl,omega,argp,nu,arglat,truelon,lonper );


        fprintf(1,' ---------------------------- begin tests ------------------------- \n' );
        fprintf(1,' ---------------------------- begin tests ------------------------- ' );
        anomeq1 = 'mean';  % true, mean
        anomeq1 = 'true';  % true, mean
        anomeq2 = 'n';     % a, n
        anomflt = 'latlon'; % latlon  radec

        [ut1, tut1, jdut1, jdut1frac, utc, tai, tt, ttt, jdtt, jdttfrac, tdb, ttdb, jdtdb, jdtdbfrac, tcg, jdtcg,jdtcgfrac, tcb, jdtcb,jdtcbfrac  ] ...
            = convtime ( year, mon, day, hr, min, sec, timezone, dut1, dat );

        % --- convert the eci state into the various other state formats (classical, equinoctial, etc)
        [cartstate,classstate,flstate,eqstate, fr] = setcov(reci,veci, ...
            year,mon,day,hr,min,sec,dut1,dat,ttt,jdut1,lod,xp,yp,terms,'y',strcat(anomeq1,anomeq2),anomflt,ddpsi,ddeps);

        fprintf(1,'==================== do the sensitivity tests \n' );
        cartcov = [ ...
            100.0,  1.0e-2, 1.0e-2, 1.0e-4,   1.0e-4,   1.0e-4; ...
            1.0e-2, 100.0,  1.0e-2, 1.0e-4,   1.0e-4,   1.0e-4; ...
            1.0e-2, 1.0e-2, 100.0,  1.0e-4,   1.0e-4,   1.0e-4; ...
            1.0e-4, 1.0e-4, 1.0e-4, 0.0001,   1.0e-6,   1.0e-6; ...
            1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6,   0.0001,   1.0e-6; ...
            1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6,   1.0e-6,   0.0001];

        fprintf(1,'1.  Cartesian Covariance \n');
        printcov( cartcov,'ct','m',strcat(anomeq1,'a') );


        % test partials
        %         % partial a wrt rx
        %         [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe  (reci,veci);
        %         magr = mag(reci);
        %         delta = reci(1) * 0.00001;
        %         reci(1) = reci(1) + delta;
        %         magr1 = mag(reci);
        %         [p,a1,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe  (reci,veci);
        %         p0 = (a-a1)/delta;
        %         p1 = 2.0*a^2*reci(1) / magr^3;
        %         fprintf(1,' a wrt rx %14.14f  %14.14f \n', p0, p1);
        %
        %         % partial n wrt rx
        %         [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe  (reci,veci);
        %         magr = mag(reci);
        %         n = sqrt(mu/a^3);
        %         recin = reci;
        %         delta = recin(1) * 0.00001;
        %         recin(1) = recin(1) + delta;
        %         magr1 = mag(recin);
        %         [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe  (recin,veci);
        %         n1 = sqrt(mu/a^3);
        %         p0 = (n-n1)/delta;
        %         p2 = -3.0*n1*a*reci(1)/magr1^3;
        %         fprintf(1,' n wrt rx %14.14f  %14.14f \n', p0, p2);
        %
        %         % partial a wrt vx
        %         [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe  (reci,veci);
        %         magr = mag(veci);
        %         vecin = veci;
        %         delta = vecin(1) * 0.00001;
        %         vecin(1) = vecin(1) + delta;
        %         magv1 = mag(vecin);
        %         [p,a1,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe  (reci,vecin);
        %         p0 = (a-a1)/delta;
        %         p1 = 2.0*veci(1) / (n^2*a);
        %         fprintf(1,' a wrt vx %14.14f  %14.14f \n', p0, p1);
        %
        %         % partial n wrt vx
        %         [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe  (reci,veci);
        %         magr = mag(veci);
        %         n = sqrt(mu/a^3);
        %         vecin = veci;
        %         delta = vecin(1) * 0.00001;
        %         vecin(1) = vecin(1) + delta;
        %         magv1 = mag(vecin);
        %         [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe  (reci,vecin);
        %         n1 = sqrt(mu/a^3);
        %         p0 = (n-n1)/delta;
        %         p2 = -3.0*vecin(1)/(n*a^2);
        %         fprintf(1,' n wrt vx %14.14f  %14.14f \n', p0, p2);
        %
        %         % partial n wrt vz
        %         [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe  (reci,veci);
        %         magr = mag(veci);
        %         n = sqrt(mu/a^3);
        %         vecin = veci;
        %         delta = vecin(3) * 0.00001;
        %         vecin(3) = vecin(3) + delta;
        %         magv1 = mag(vecin);
        %         [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe  (reci,vecin);
        %         n1 = sqrt(mu/a^3);
        %         p0 = (n-n1)/delta;
        %         p2 = -3.0*vecin(3)/(n*a^2);
        %         fprintf(1,' n wrt vz %14.14f  %14.14f \n', p0, p2);
        %
        %         % partial rx wrt a
        %         [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe  (reci,veci);
        %         delta = a * 0.00001;
        %         a = a + delta;
        %         p = a*(1-ecc^2);
        %         [reci1, veci1] = coe2rv(p, ecc, incl, omega, argp, nu, arglat, truelon, lonper);
        %         p0 = (reci(1)-reci1(1))/delta;
        %         p1 = reci(1) / a;
        %         fprintf(1,' rx wrt a %14.14f  %14.14f \n', p0, p1);
        %         % partial rx wrt n
        %         [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe  (reci,veci);
        %         n = sqrt(mu/a^3);
        %         delta = n * 0.00001;
        %         n = n + delta;
        %         a = (mu/n^2)^(1/3);
        %         p = a*(1-ecc^2);
        %         [reci1, veci1] = coe2rv(p, ecc, incl, omega, argp, nu, arglat, truelon, lonper);
        %         p0 = (reci(1)-reci1(1))/delta;
        %         p1 = -2*reci(1) / (3*n);
        %         fprintf(1,' rx wrt n %14.14f  %14.14f \n', p0, p1);
        %
        %         % partial vx wrt a
        %          [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe  (reci,veci);
        %          delta = a * 0.00001;
        %          a = a + delta;
        %          p = a*(1-ecc^2);
        %          [reci1, veci1] = coe2rv(p, ecc, incl, omega, argp, nu, arglat, truelon, lonper);
        %          p0 = (veci(1)-veci1(1))/delta;
        %          p1 = -veci(1) / (2*a);
        %          fprintf(1,' vx wrt a %14.14f  %14.14f \n', p0, p1);
        %          % partial vx wrt n
        %          [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe  (reci,veci);
        %          n = sqrt(mu/a^3);
        %          delta = n * 0.00001;
        %          n = n + delta;
        %          a = (mu/n^2)^(1/3);
        %          p = a*(1-ecc^2);
        %          [reci1, veci1] = coe2rv(p, ecc, incl, omega, argp, nu, arglat, truelon, lonper);
        %          p0 = (veci(1)-veci1(1))/delta;
        %          p1 = veci(1) / (3*n);
        %          fprintf(1,' vx wrt n %14.14f  %14.14f \n', p0, p1);
        %
        %         % partial vz wrt a
        %         [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe  (reci,veci);
        %         delta = a * 0.00001;
        %         a1 = a + delta;
        %         p = a1*(1-ecc^2);
        %         [recin,vecin] = coe2rv ( p,ecc,incl,omega,argp,nu,0.0,0.0,0.0 );
        %         n1 = sqrt(mu/a1^3);
        %         p0 = (veci(3)-vecin(3))/delta;
        %         p2 = -veci(3)/(2*a);
        %         fprintf(1,' vz wrt a %14.14f  %14.14f \n', p0, p2);
        %         % partial vz wrt n
        %         [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe  (reci,veci);
        %         n = sqrt(mu/a^3);
        %         delta = n * 0.00001;
        %         n1 = n + delta;
        %         a1 = (mu/n1^2)^(1/3);
        %         p = a1*(1-ecc^2);
        %         [recin,vecin] = coe2rv ( p,ecc,incl,omega,argp,nu,0.0,0.0,0.0 );
        %         p0 = (veci(3)-vecin(3))/delta;
        %         p2 = veci(3)/(3*n);
        %         fprintf(1,' vz wrt n %14.14f  %14.14f \n', p0, p2);
        %         % partial lat wrt rx
        %         %[p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe  (reci,veci);
        %         delta = flstate(2) * 0.0000001;
        %         latgc1 = flstate(2) + delta;
        %         magr = flstate(5);
        %             recef(1) = magr*0.001*cos(latgc1)*cos(flstate(1));  % in km
        %             recef(2) = magr*0.001*cos(latgc1)*sin(flstate(1));
        %             recef(3) = magr*0.001*sin(latgc1);
        %             vecef = [0; 0; 0];
        %             aecef = [0;0;0];
        %             [recin,vecin,a] = ecef2eci(recef',vecef,aecef,ttt,jdut1,lod,xp,yp,terms,ddpsi,ddeps);
        %
        %         n = sqrt(mu/a^3);
        %         a1 = (mu/n1^2)^(1/3);
        %         p = a1*(1-ecc^2);
        %         [recin,vecin] = coe2rv ( p,ecc,incl,omega,argp,nu,0.0,0.0,0.0 );
        %         p0 = (reci(1)-recin(1))/delta;
        %         p1 = -magr*sin(flstate(1))*cos(flstate(2));
        %         p2 = -reci(2)/sqrt(reci(1)^2 + reci(2)^2);
        %         fprintf(1,' lat wrt rx %14.14f  %14.14f \n', p0, p2);
        %  pause

        % paper approach
        % ===============================================================================================


        fprintf(1,'===============================================================================================\n');
        fprintf(1,'1.  RSW and NTW Covariance from Cartesian #1 above ------------------- \n');
        [covrsw,tm] = covct2rsw(cartcov,cartstate);
        fprintf(1,'rsw\n');
        printcov( covrsw,'ct','m',anom );
        temt = covrsw;

        [covntw,tm] = covct2ntw(cartcov,cartstate);
        fprintf(1,'ntw\n');
        printcov( covntw,'ct','m',anom );
        temt = covntw;

        pause;


        fprintf(1,'===============================================================================================\n');
        % --- convert the eci state into the various other state formats (classical, equinoctial, etc)
        anom = 'meana';
        [cartstate,classstate,flstate,eqstate, fr] = setcov(reci,veci, ...
            year,mon,day,hr,min,sec,dut1,dat,ttt,jdut1,lod,xp,yp,terms,'y',anom,anomflt,ddpsi,ddeps);
        fprintf(1,'2.  Classical Covariance from Cartesian #1 above (meana) ------------------- \n');
        [classcovmeana,tmct2cl] = covct2clnew( cartcov, cartstate,anom );
        printcov( classcovmeana,'cl','m',anom );


        fprintf(1,'  Cartesian Covariance from Classical #2 above \n');
        [cartcovmeanarev, tmcl2ct]   = covcl2ctnew( classcovmeana, classstate,anom );
        printcov( cartcovmeanarev,'ct','m',anom );
        fprintf(1,'\n');

        printdiff( ' cartcov - cartcovmeanarev \n', cartcov, cartcovmeanarev);

        printcov( tmct2cl*tmcl2ct,'tm','m',anom );
        %printdiff( ' tmct2cl - inv(tmcl2ct) \n', tmct2cl, inv(tmcl2ct));
        %rintdiff( ' tmcl2ct - inv(tmct2cl) \n', tmcl2ct, inv(tmct2cl));
        pause;

        % ===============================================================================================
        fprintf(1,'2.  Classical Covariance from Cartesian #1 above (meann) ------------------- \n');
        anom = 'meann';
        [cartstate,classstate,flstate,eqstate, fr] = setcov(reci,veci, ...
            year,mon,day,hr,min,sec,dut1,dat,ttt,jdut1,lod,xp,yp,terms,'y',anom,anomflt,ddpsi,ddeps);
        [classcovmeann,tmct2cl] = covct2clnew( cartcov,cartstate,anom );
        printcov( classcovmeann,'cl','m',anom );

        fprintf(1,'  Cartesian Covariance from Classical #2 above \n');
        [cartcovmeannrev, tmcl2ct]   = covcl2ctnew( classcovmeann,classstate,anom );
        printcov( cartcovmeannrev,'ct','m',anom );
        fprintf(1,'\n');
        %         fprintf(1,'-------- tm cl2ct new ---------\n');
        %         printcov( tmcl2ct,'tm','m','meann' );
        %         fprintf(1,'-------- tm ct2cl new ---------\n');
        %         printcov( tmct2cl,'tm','m','meann' );
        %         fprintf(1,'\n');

        printdiff( ' cartcov - cartcovmeannrev \n', cartcov, cartcovmeannrev);
        printcov( tmct2cl*tmcl2ct,'tm','m',anom );
        %printdiff( ' tmct2cl - inv(tmcl2ct) \n', tmct2cl, inv(tmcl2ct));
        %printdiff( ' tmcl2ct - inv(tmct2cl) \n', tmcl2ct, inv(tmct2cl));

        pause;

        % ===============================================================================================
        fprintf(1,'2.  Classical Covariance from Cartesian #1 above (truea) -------------------- \n');
        anom = 'truea';
        [cartstate,classstate,flstate,eqstate, fr] = setcov(reci,veci, ...
            year,mon,day,hr,min,sec,dut1,dat,ttt,jdut1,lod,xp,yp,terms,'y',anom,anomflt,ddpsi,ddeps);
        [classcovtruea,tmct2cl] = covct2clnew( cartcov,cartstate,anom );
        printcov( classcovtruea,'cl','m',anom );

        fprintf(1,'  Cartesian Covariance from Classical #2 above \n');
        [cartcovtruearev, tmcl2ct]   = covcl2ctnew( classcovtruea,classstate,anom );
        printcov( cartcovtruearev,'ct','m',anom );
        fprintf(1,'\n');

        tmcl2cttruea = tmcl2ct;
        
        printdiff( ' cartcov - cartcovtruearev \n', cartcov, cartcovtruearev);
        printcov( tmct2cl*tmcl2ct,'tm','m',anom );
        %printdiff( ' tmct2cl - inv(tmcl2ct) \n', tmct2cl, inv(tmcl2ct));
        %printdiff( ' tmcl2ct - inv(tmct2cl) \n', tmcl2ct, inv(tmct2cl));

        pause;

        % ===============================================================================================
        fprintf(1,'2.  Classical Covariance from Cartesian #1 above (truen) -------------------- \n');
        anom = 'truen';
        [cartstate,classstate,flstate,eqstate, fr] = setcov(reci,veci, ...
            year,mon,day,hr,min,sec,dut1,dat,ttt,jdut1,lod,xp,yp,terms,'y',anom,anomflt,ddpsi,ddeps);
        [classcovtruen,tmct2cl] = covct2clnew( cartcov,cartstate,anom );
        printcov( classcovtruen,'cl','m',anom );


        fprintf(1,'  Cartesian Covariance from Classical #2 above \n');
        [cartcovtruenrev, tmcl2ct]   = covcl2ctnew( classcovtruen,classstate,anom );
        printcov( cartcovtruenrev,'ct','m',anom );
        fprintf(1,'\n');

        tmcl2cttruen = tmcl2ct;
  
        printdiff( ' cartcov - cartcovtruenrev \n', cartcov, cartcovtruenrev);
        printcov( tmct2cl*tmcl2ct,'tm','m',anom );
        %printdiff( ' tmct2cl - inv(tmcl2ct) \n', tmct2cl, inv(tmcl2ct));
        %printdiff( ' tmcl2ct - inv(tmct2cl) \n', tmcl2ct, inv(tmct2cl));

        pause;
        % ===============================================================================================
        fprintf(1,'===============================================================================================\n');
        % strcat(anomeq1,anomeq2)
        fprintf(1,'3.  Equinoctial Covariance from Classical #2 above (truea) \n');
        anom = 'truea';
        [cartstate,classstate,flstate,eqstate, fr] = setcov(reci,veci, ...
            year,mon,day,hr,min,sec,dut1,dat,ttt,jdut1,lod,xp,yp,terms,'y',anom,anomflt,ddpsi,ddeps);
        [classcovtruea,tmct2cl] = covct2clnew( cartcov,cartstate,anom );
        [eqcovtruea, tmcl2eq]    = covcl2eq( classcovtruea,classstate,anom, fr );
        printcov( eqcovtruea,'eq','m',anom );
        %eqcov = eqcovtruea; % save for later

        tmct2eqtruea = tmct2cl*tmcl2eq;

        fprintf(1,'4.  Classical Covariance from Equinoctial #3 above \n');
        [classcovtruearev,tmeq2cl] = coveq2cl( eqcovtruea,eqstate,anom, fr );
        printcov( classcovtruearev,'cl','m',anom );

        tmeq2cttruea = tmeq2cl*tmcl2cttruea;

        printdiff( ' classcovtruea - classcovtruearev \n', classcovtruea, classcovtruearev);
        printcov( tmcl2eq*tmeq2cl,'tm','m',anom );
        %printdiff( ' tmcl2eq - inv(tmeq2cl) \n', tmcl2eq, inv(tmeq2cl));
        %printdiff( ' tmeq2cl - inv(tmcl2eq) \n', tmeq2cl, inv(tmcl2eq));

        pause;

        % ===============================================================================================
        % strcat(anomeq1,anomeq2)
        fprintf(1,'3.  Equinoctial Covariance from Classical #2 above (truen) \n');
        anom = 'truen';
        [cartstate,classstate,flstate,eqstate, fr] = setcov(reci,veci, ...
            year,mon,day,hr,min,sec,dut1,dat,ttt,jdut1,lod,xp,yp,terms,'y',anom,anomflt,ddpsi,ddeps);
        [classcovtruen,tmct2cl] = covct2clnew( cartcov,cartstate,anom );
        [eqcovtruen, tmcl2eq]    = covcl2eq( classcovtruen,classstate,anom, fr );
        printcov( eqcovtruen,'eq','m',anom );
        %eqcov = eqcovtruen; % save for later

        tmct2eqtruen = tmcl2eq*tmct2cl;

        fprintf(1,'4.  Classical Covariance from Equinoctial #3 above \n');
        [classcovtruenrev,tmeq2cl] = coveq2cl( eqcovtruen,eqstate,anom, fr );
        printcov( classcovtruenrev,'cl','m',anom );

        tmeq2cttruen = tmcl2cttruen*tmeq2cl;

        printdiff( ' classcovtruen - classcovtruenrev \n', classcovtruen, classcovtruenrev);
        printcov( tmcl2eq*tmeq2cl,'tm','m',anom );
        %printdiff( ' tmcl2eq - inv(tmeq2cl) \n', tmcl2eq, inv(tmeq2cl));
        %printdiff( ' tmeq2cl - inv(tmcl2eq) \n', tmeq2cl, inv(tmcl2eq));

        pause;

        % ===============================================================================================
        % strcat(anomeq1,anomeq2)
        fprintf(1,'3.  Equinoctial Covariance from Classical #2 above (meana) \n');
        anom = 'meana';
        [cartstate,classstate,flstate,eqstate, fr] = setcov(reci,veci, ...
            year,mon,day,hr,min,sec,dut1,dat,ttt,jdut1,lod,xp,yp,terms,'y',anom,anomflt,ddpsi,ddeps);
        [classcovmeana,tmct2cl] = covct2clnew( cartcov,cartstate,anom );
        [eqcovmeana, tmcl2eq]    = covcl2eq( classcovmeana,classstate,anom, fr );
        printcov( eqcovmeana,'eq','m',anom );

        fprintf(1,'4.  Classical Covariance from Equinoctial #3 above \n');
        [classcovmeanarev,tmeq2cl] = coveq2cl( eqcovmeana,eqstate,anom, fr );
        printcov( classcovmeanarev,'cl','m',anom );

        printdiff( ' classcovmeana - classcovmeanarev \n', classcovmeana, classcovmeanarev);
        printcov( tmcl2eq*tmeq2cl,'tm','m',anom );
        %printdiff( ' tmcl2eq - inv(tmeq2cl) \n', tmcl2eq, inv(tmeq2cl));
        %printdiff( ' tmeq2cl - inv(tmcl2eq) \n', tmeq2cl, inv(tmcl2eq));

        pause;

        % ===============================================================================================
        % strcat(anomeq1,anomeq2)
        fprintf(1,'3.  Equinoctial Covariance from Classical #2 above (meann) \n');
        anom = 'meann';
        [cartstate,classstate,flstate,eqstate, fr] = setcov(reci,veci, ...
            year,mon,day,hr,min,sec,dut1,dat,ttt,jdut1,lod,xp,yp,terms,'y',anom,anomflt,ddpsi,ddeps);
        [classcovmeann,tmct2cl] = covct2clnew( cartcov,cartstate,anom );
        [eqcovmeann, tmcl2eq]    = covcl2eq( classcovmeann,classstate,anom, fr );
        printcov( eqcovmeann,'eq','m',anom );

        fprintf(1,'4.  Classical Covariance from Equinoctial #3 above \n');
        [classcovmeannrev,tmeq2cl] = coveq2cl( eqcovmeann,eqstate,anom, fr );
        printcov( classcovmeannrev,'cl','m',anom );

        printdiff( ' classcovmeann - classcovmeannrev \n', classcovmeann, classcovmeannrev);
        printcov( tmcl2eq*tmeq2cl,'tm','m',anom );
        %printdiff( ' tmcl2eq - inv(tmeq2cl) \n', tmcl2eq, inv(tmeq2cl));
        %printdiff( ' tmeq2cl - inv(tmcl2eq) \n', tmeq2cl, inv(tmcl2eq));

        pause;

        % ===============================================================================================
        fprintf(1,'===============================================================================================\n');
        fprintf(1,'5.  Equinoctial Covariance from Cartesian #1 above (truen) \n');
        anom = 'truen';
        [cartstate,classstate,flstate,eqstate, fr] = setcov(reci,veci, ...
            year,mon,day,hr,min,sec,dut1,dat,ttt,jdut1,lod,xp,yp,terms,'y',anom,anomflt,ddpsi,ddeps);
        [eqcovDtruen, tmct2eq]   = covct2eq( cartcov,cartstate,anom, fr );
        printcov( eqcovDtruen,'eq','m',anom );

        printdiff( ' eqcovtruen - eqcovDtruen \n', eqcovtruen, eqcovDtruen);

        fprintf(1,'6.  Cartesian Covariance from Equinoctial #5 above \n');
        % try with intermediate one
         eqcovDtruenx = [  4.3069707220e-17    1.2444284692e-14    1.4014592713e-14   -4.7096511176e-17   -5.1804415247e-17    2.6405645277e-17
             1.2444284692e-14    6.1777913966e-12    2.6215113639e-12   -2.2655462787e-14   -2.3898379798e-14   -1.5822346004e-12
             1.4014592713e-14    2.6215113639e-12    6.7713072988e-12   -1.8031837762e-14   -1.6405199435e-14    1.4140513359e-12
             -4.7096511176e-17   -2.2655462787e-14   -1.8031837762e-14    2.5193704546e-12   -2.7497854222e-13    7.2254489606e-13
             -5.1804415247e-17   -2.3898379798e-14   -1.6405199435e-14   -2.7497854222e-13    2.5859832833e-12   -2.5681926955e-12
             2.6405645280e-17   -1.5822346004e-12    1.4140513359e-12    7.2254489606e-13   -2.5681926955e-12    4.7583323083e-12];

        [cartcovDtruenrev,tmeq2cti] = coveq2ct( eqcovDtruen, eqstate, anom, fr );
        printcov( cartcovDtruenrev,'ct','m',anom );

        printdiff( ' cartcov - cartcovDtruenrev \n', cartcov, cartcovDtruenrev);
        printcov( tmcl2eq*tmeq2cl,'tm','m',anom );
        %printdiff( ' tmct2eq - inv(tmeq2cti) \n', tmct2eq, inv(tmeq2cti));
        %printdiff( ' tmeq2cti - inv(tmct2eq) \n', tmeq2cti, inv(tmct2eq));

        % compare combined tm 
        fprintf(1,'===============================================================================================\n');
        % fwd
        printcov( tmct2eqtruen,'tm','m',anom );
        fprintf(1,' \n');
        printcov( tmct2eq,'tm','m',anom );
        printdiff( ' tmct2eqtruen - tmct2eq \n', tmct2eqtruen, tmct2eq);
  
        fprintf(1,'===============================================================================================\n');
        % bacwd
        printcov( tmeq2cttruen,'tm','m',anom );
        fprintf(1,' \n');
        printcov( tmeq2cti,'tm','m',anom );
        printdiff( ' tmeq2cttruen - tmeq2cti \n', tmeq2cttruen, tmeq2cti);
        
        pause;

        % ===============================================================================================
        fprintf(1,'===============================================================================================\n');
        fprintf(1,'5.  Equinoctial Covariance from Cartesian #1 above (meana) \n');
        anom = 'meana';
        [cartstate,classstate,flstate,eqstate, fr] = setcov(reci,veci, ...
            year,mon,day,hr,min,sec,dut1,dat,ttt,jdut1,lod,xp,yp,terms,'y',anom,anomflt,ddpsi,ddeps);
        [eqcovDmeana, tmct2eq]   = covct2eq( cartcov,cartstate,anom, fr );
        printcov( eqcovDmeana,'eq','m',anom );

        printdiff( ' eqcovmeana - eqcovDmeana \n', eqcovmeana, eqcovDmeana);

        fprintf(1,'6.  Cartesian Covariance from Equinoctial #5 above \n');
        % try with intermediate one
        [cartcovDmeanarev,tmeq2cti] = coveq2ct( eqcovDmeana, eqstate, anom, fr );
        printcov( cartcovDmeanarev,'ct','m',anom );

        printdiff( ' cartcov - cartcovDmeanarev \n', cartcov, cartcovDmeanarev);
        printcov( tmct2eq*tmeq2cti,'tm','m',anom );
        %printdiff( ' tmct2eq - inv(tmeq2cti) \n', tmct2eq, inv(tmeq2cti));
        %printdiff( ' tmeq2cti - inv(tmct2eq) \n', tmeq2cti, inv(tmct2eq));

        pause;

          % ===============================================================================================
        fprintf(1,'===============================================================================================\n');
        fprintf(1,'7.  Flight Covariance from Cartesian #1 above \n');
        anom = 'meana';
        anomflt = 'latlon';
        [cartstate,classstate,flstate,eqstate, fr] = setcov(reci,veci, ...
            year,mon,day,hr,min,sec,dut1,dat,ttt,jdut1,lod,xp,yp,terms,'y',anom,anomflt,ddpsi,ddeps);
        [flcov, tmct2fl] = covct2fl( cartcov,cartstate, anomflt, ttt,jdut1,lod,xp,yp,2,ddpsi,ddeps);
        if strcmp(anomflt,'latlon') == 1  % 1 is true
            printcov( flcov,'fl','m',anomflt);
        else
            printcov( flcov,'sp','m',anomflt );
        end

        fprintf(1,'8. Cartesian Covariance from Flight #7 above \n');
        [cartcovfltrev, tmfl2ct] = covfl2ct( flcov,flstate,anomflt, ttt,jdut1,lod,xp,yp,2,ddpsi,ddeps );
        printcov( cartcovfltrev,'ct','m',anom );

        printdiff( ' cartcov - cartcovfltrev \n', cartcov, cartcovfltrev);

        fprintf(1,'9. Classical Covariance from Flight #7 above \n');
        fprintf(1,'\n-------- tm fl2cl --------- \n');
        [classcovfl, tmfl2cl] = covfl2cltest( flcov,flstate,anomflt, ttt,jdut1,lod,xp,yp,2,ddpsi,ddeps );
        printcov( classcovfl,'cl','m',anom );

        printdiff( ' classcov - classcovfl \n', classcovmeana, classcovfl);

        printcov( tmct2fl*tmfl2ct,'tm','m',anom );

          % ===============================================================================================
        fprintf(1,'===============================================================================================\n');
        fprintf(1,'7.  Flight Covariance from Cartesian #1 above \n');
        anom = 'meana';
        anomflt = 'radec';
        [cartstate,classstate,flstate,eqstate, fr] = setcov(reci,veci, ...
            year,mon,day,hr,min,sec,dut1,dat,ttt,jdut1,lod,xp,yp,terms,'y',anom,anomflt,ddpsi,ddeps);
        [flcov, tmct2fl] = covct2fl( cartcov,cartstate, anomflt, ttt,jdut1,lod,xp,yp,2,ddpsi,ddeps);
        if strcmp(anomflt,'latlon') == 1  % 1 is true
            printcov( flcov,'fl','m',anomflt );
        else
            printcov( flcov,'sp','m',anomflt );
        end

        fprintf(1,'8. Cartesian Covariance from Flight #7 above \n');
        [cartcovfltrev, tmfl2ct] = covfl2ct( flcov,flstate,anomflt, ttt,jdut1,lod,xp,yp,2,ddpsi,ddeps );
        printcov( cartcovfltrev,'ct','m',anom );

        printdiff( ' cartcov - cartcovfltrev \n', cartcov, cartcovfltrev);

        fprintf(1,'9. Classical Covariance from Flight #7 above \n');
        fprintf(1,'\n-------- tm fl2cl --------- \n');
        [classcovfl, tmfl2cl] = covfl2cltest( flcov,flstate,anomflt, ttt,jdut1,lod,xp,yp,2,ddpsi,ddeps );
        printcov( classcovfl,'cl','m',anom );

        printdiff( ' classcov - classcovfl \n', classcovmeana, classcovfl);


 
        % temp testing !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        %
        %         fprintf(1,'2.temp testing!!  find original transformations \n');
        %         [classco, tmct2clO]  = covct2clnewO( cartcov,cartstate,strcat(anomeq1,'a') );
        %         [cartcov1, tmcl2ctO] = covcl2ctnewO( classco,classstate,strcat(anomeq1,'a') );
        %
        %         fprintf(1,'-------- tm cl2ct old ---------\n');
        %         printcov( tmcl2ctO,'tm','m',strcat(anomeq1,'a') );
        %         fprintf(1,'-------- tm ct2cl old ---------\n');
        %         printcov( tmct2clO,'tm','m',strcat(anomeq1,'a') );
        %
        %         fprintf(1,'Check consistency of both approaches tmct2cl-inv(tmcl2ct) diff pct over 1e-18 \n');
        %         fprintf(1,'-------- accuracy of tm comparing ct2cl and cl2ct --------- \n');
        %         tm1 = tmct2clO;
        %         tm2 = inv(tmcl2ctO);
        %         for i=1:6
        %             for j = 1:6
        %                 if (abs( tm1(i,j) - tm2(i,j) ) < small) || (abs(tm1(i,j)) < small)
        %                     diffm(i,j) = 0.0;
        %                   else
        %                     diffm(i,j) = 100.0*( (tm1(i,j)-tm2(i,j)) / tm1(i,j));
        %                 end;
        %             end;
        %         end;
        %         fprintf(1,'pct differences if over %4e \n',small);
        %         fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',diffm' );
        % temp testing !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        pause;



        fprintf(1,'-------- accuracy of tm comparing cl2eq and eq2cl --------- \n');
        tm1 = tmcl2eq;
        tm2 = inv(tmeq2cl);
        for i=1:6
            for j = 1:6
                if (abs( tm1(i,j) - tm2(i,j) ) < small) || (abs(tm1(i,j)) < small)
                    diffmm(i,j) = 0.0;
                else
                    diffmm(i,j) = 100.0*( (tm1(i,j)-tm2(i,j)) / tm1(i,j));
                end;
            end;
        end;
        fprintf(1,'pct differences if over %4e \n',small);
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',diffmm' );


        fprintf(1,'-------- accuracy of tm comparing ct2eq and eq2ct --------- \n');
        tm1 = tmct2eq;
        tm2 = inv(tmeq2ct);
        for i=1:6
            for j = 1:6
                if (abs( tm1(i,j) - tm2(i,j) ) < small) || (abs(tm1(i,j)) < small)
                    diffmm(i,j) = 0.0;
                else
                    diffmm(i,j) = 100.0*( (tm1(i,j)-tm2(i,j)) / tm1(i,j));
                end;
            end;
        end;
        fprintf(1,'pct differences if over %4e \n',small);
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',diffmm' );


        fprintf(1,'-------- accuracy of tm comparing ct2eq and eq2ctintermediate --------- \n');
        tm1 = tmct2eq;
        tm2 = inv(tmeq2cti);
        for i=1:6
            for j = 1:6
                if (abs( tm1(i,j) - tm2(i,j) ) < small) || (abs(tm1(i,j)) < small)
                    diffmm(i,j) = 0.0;
                else
                    diffmm(i,j) = 100.0*( (tm1(i,j)-tm2(i,j)) / tm1(i,j));
                end;
            end;
        end;
        fprintf(1,'pct differences if over %4e \n',small);
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',diffmm' );


        fprintf(1,'-------- accuracy of tm comparing cl2fl and fl2cl --------- \n');
        tm1 = tmct2fl;
        tm2 = inv(tmfl2ct);
        for i=1:6
            for j = 1:6
                if (abs( tm1(i,j) - tm2(i,j) ) < small) || (abs(tm1(i,j)) < small)
                    diffmm(i,j) = 0.0;
                else
                    diffmm(i,j) = 100.0*( (tm1(i,j)-tm2(i,j)) / tm1(i,j));
                end;
            end;
        end;
        fprintf(1,'pct differences if over %4e \n',small);
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',diffmm' );



        temp = tmcl2eq * tmct2cl;
        temp1 = tmcl2ct * tmeq2cl;

        fprintf(1,'\n-------- tm combined ct2cl, cl2eq --------- \n');
        printcov( temp,'tm','m',strcat(anomeq1,anomeq2) );
        fprintf(1,'\n-------- tm ct2eq --------- \n');
        printcov( tmct2eq,'tm','m',strcat(anomeq1,anomeq2) );

        fprintf(1,'\n-------- tm combined eq2cl, cl2ct --------- \n');
        printcov( temp1,'tm','m',strcat(anomeq1,anomeq2) );
        fprintf(1,'\n-------- tm ct2eq --------- \n');
        printcov( tmeq2ct,'tm','m',strcat(anomeq1,anomeq2) );

        fprintf(1,'-------- accuracy of test tm ct2eq --------- \n');
        tm1 = temp;
        tm2 = tmct2eq;
        for i=1:6
            for j = 1:6
                if (abs( tm1(i,j) - tm2(i,j) ) < small) || (abs(tm1(i,j)) < small)
                    diffmm(i,j) = 0.0;
                else
                    diffmm(i,j) = 100.0*( (tm1(i,j)-tm2(i,j)) / tm1(i,j));
                end;
            end;
        end;
        fprintf(1,'pct differences if over %4e \n',small);
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',diffmm' );

        fprintf(1,'-------- accuracy of test tm eq2ct --------- \n');
        tm1 = temp1;
        tm2 = tmeq2ct;
        for i=1:6
            for j = 1:6
                if (abs( tm1(i,j) - tm2(i,j) ) < small) || (abs(tm1(i,j)) < small)
                    diffmm(i,j) = 0.0;
                else
                    diffmm(i,j) = 100.0*( (tm1(i,j)-tm2(i,j)) / tm1(i,j));
                end;
            end;
        end;
        fprintf(1,'pct differences if over %4e \n',small);
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',diffmm' );


        pause;
        %anom = 'true';
        [classcovrev, tm] = covct2clnew( cartcov,cartstate,strcat(anomeq1,anomeq2) );
        printcov( classcovrev,'cl','m',strcat(anomeq1,anomeq2) );

        %        [cartcov1, tm]   = covcl2ct( classco1,classstate,strcat(anomeq1,anomeq2) );
        %        printcov( cartcov1,'ct','m',strcat(anomeq1,anomeq2) );


        pause;

        fprintf(1,'===================== do the eq-ct-eq tests \n' );
        eqcov = [ ...
            1.0e-14, 1.0e-16, 1.0e-16, 1.0e-16, 1.0e-16, 1.0e-16; ...
            1.0e-16, 1.0e-14, 1.0e-16, 1.0e-16, 1.0e-16, 1.0e-16; ...
            1.0e-16, 1.0e-16, 1.0e-14, 1.0e-16, 1.0e-16, 1.0e-16; ...
            1.0e-16, 1.0e-16, 1.0e-16, 1.0e-19, 1.0e-16, 1.0e-16; ...
            1.0e-16, 1.0e-16, 1.0e-16, 1.0e-16, 1.0e-14, 1.0e-16; ...
            1.0e-16, 1.0e-16, 1.0e-16, 1.0e-16, 1.0e-16, 1.0e-14];
        printcov( eqcov,'eq','m',anom );
        [classco,tm] = coveq2cl( eqcov,eqstate,anom, fr);
        [cartco]  = covcl2ctnew( classco,classstate,anom );
        [classco,tm] = covct2clnew( cartco,cartstate,anom );
        [eqcovmeannrev]    = covcl2eq( classco,classstate,anom, fr );
        printcov( eqcovmeannrev,'eq','m',anom );
        for i=1:6
            for j = 1:6
                if (abs( eqcov(i,j)-eqcovmeannrev(i,j) ) < small) || (abs(eqcov(i,j)) < small)
                    diffmm(i,j) = 0.0;
                else
                    diffmm(i,j) = 100.0*( (eqcov(i,j)-eqcovmeannrev(i,j)) / eqcov(i,j));
                end;
            end;
        end;
        fprintf(1,'-------- accuracy --------- \n');
        fprintf(1,'pct differences if over %4e \n',small);
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',diffmm' );


        % ---------------- reset original cartcov ----------------------------
        cartcov = [ ...
            1.0, 0.0, 0.0, 0.0, 0.0, 0.0; ...
            0.0, 1.0, 0.0, 0.0, 0.0, 0.0; ...
            0.0, 0.0, 1.0, 0.0, 0.0, 0.0; ...
            0.0, 0.0, 0.0, 0.000001, 0.0, 0.0; ...
            0.0, 0.0, 0.0, 0.0, 0.000001, 0.0; ...
            0.0, 0.0, 0.0, 0.0, 0.0, 0.000001];
        cartcov = [ ...
            0.81, 1.0e-8, 1.0e-8, 1.0e-8, 1.0e-8, 1.0e-8; ...
            1.0e-8, 0.81, 1.0e-8, 1.0e-8, 1.0e-8, 1.0e-8; ...
            1.0e-8, 1.0e-8, 0.81, 1.0e-8, 1.0e-8, 1.0e-8; ...
            1.0e-8, 1.0e-8, 1.0e-8, 0.000001, 1.0e-8, 1.0e-8; ...
            1.0e-8, 1.0e-8, 1.0e-8, 1.0e-8, 0.000001, 1.0e-8; ...
            1.0e-8, 1.0e-8, 1.0e-8, 1.0e-8, 1.0e-8, 0.000001];
        cartcov = [ ...
            100.0,    1.0e-2, 1.0e-2, 1.0e-4,   1.0e-4,   1.0e-4; ...
            1.0e-2, 100.0,    1.0e-2, 1.0e-4,   1.0e-4,   1.0e-4; ...
            1.0e-2, 1.0e-2, 100.0,    1.0e-4,   1.0e-4,   1.0e-4; ...
            1.0e-4, 1.0e-4, 1.0e-4, 0.0001, 1.0e-6,   1.0e-6; ...
            1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6,   0.0001, 1.0e-6; ...
            1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6,   1.0e-6,   0.0001];

        % -------------------------------------------------------------
        % ------------- start covariance conversion tests -------------
        fprintf(1,'anomaly is %s \n',anom );
        fprintf(1,'initial cartesian covariance \n');
        printcov( cartcov,'ct','m',anom );

        fprintf(1,' -------- classical covariance conversions --------- \n');
        fprintf(1,' -----  cartesian to classical covariance  --------- \n');
        [classcov,tm] = covct2clnew(cartcov,cartstate,anom );
        printcov( classcov,'cl','m',anom );

        fprintf(1,'tm ct2cl \n' );
        fprintf(1,'%16e%16e%16e%16e%16e%16e\n',tm' );
        fprintf(1,'tm ct2cl inv \n' );
        fprintf(1,'%16e%16e%16e%16e%16e%16e\n',inv(tm)' );
        tmold = inv(tm);

        %        fprintf(1,'Check the orthogonality (mat*transpose) of tm \n' );
        %        fprintf(1,'%16.12f%16.12f%16.12f%16.12f%16.12f%16.12f\n',(tm * tm')' );

        %        fprintf(1,'Check the xxxx(transpose*mat) of classcov \n' );
        %        fprintf(1,'%16.12f%16.12f%16.12f%16.12f%16.12f%16.12f\n',orth(classcov)' );
        %        fprintf(1,'Check the xxxx(mat*transpose) of cartcov \n' );
        %        fprintf(1,'%16.12f%16.12f%16.12f%16.12f%16.12f%16.12f\n',orth(cartcov)' );

        %        % --- now check out what the scaling should be beteween
        %        for i=1:6
        %          for j = 1:6
        %            diffm(i,j) = classcovtrace(i,j)/classcov(i,j);
        %           end;
        %         end;
        %        fprintf(1,'scaling factors - should be \n');
        %        fprintf(1,'%16e%16e%16e%16e%16e%16e\n',diffm' );
        %        for i=1:6
        %           for j = 1:6
        %                if (abs( classcovt(i,j)-classcov(i,j) ) < small) | (abs(classcovt(i,j)) < small)
        %                    diffm(i,j) = 0.0;
        %                  else
        %                    diffm(i,j) = 100.0*( (classcovt(i,j)-classcov(i,j)) / classcovt(i,j));
        %                  end;
        %              end;
        %          end;
        %
        %        % diff just to trace output
        %        fprintf(1,'-------- accuracy to trace --------- \n');
        %        fprintf(1,'pct differences if over %4e \n', small);
        %        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',diffm' );

        fprintf(1,'----- classical to cartesian covariance  ---------- \n');
        [cartco,tm] = covcl2ctnew( classcov,classstate,anom );
        printcov( cartco,'ct','m',anom );

        fprintf(1,'tm cl2ct \n' );
        fprintf(1,'%16e%16e%16e%16e%16e%16e\n',tm' );

        %        fprintf(1,'Check the orthogonality (mat*transpose) of tm \n' );
        %        fprintf(1,'%16e%16e%16e%16e%16e%16e\n',(tm*tm')' );

        for i=1:6
            for j = 1:6
                if (abs( cartcov(i,j)-cartco(i,j) ) < small) || (abs(cartcov(i,j)) < small)
                    diffmm(i,j) = 0.0;
                else
                    diffmm(i,j) = 100.0*( (cartcov(i,j)-cartco(i,j)) / cartcov(i,j));
                end;
            end;
        end;
        fprintf(1,'-------- accuracy --------- \n');
        fprintf(1,'pct differences if over %4e \n',small);
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',diffmm' );

        for i=1:6
            for j = 1:6
                if (abs( tmold(i,j)-tm(i,j) ) < small) || (abs(tmold(i,j)) < small)
                    diffmm(i,j) = 0.0;
                else
                    diffmm(i,j) = 100.0*( (tmold(i,j)-tm(i,j)) / tmold(i,j));
                end;
            end;
        end;
        fprintf(1,'-------- tm accuracy --------- \n');
        fprintf(1,'pct differences if over %4e \n',small);
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',diffmm' );
        pause;
        fprintf(1,' -------- flight covariance conversions --------- \n');
        fprintf(1,' -----  cartesian to flight covariance  --------- \n');
        [flcov,tm] = covct2fl( cartcov,cartstate, anom, ttt,jdut1,lod,xp,yp,terms,ddpsi,ddeps);
        printcov( flcov,'fl','m',anom );

        fprintf(1,'tm ct2fl \n' );
        fprintf(1,'%16e%16e%16e%16e%16e%16e\n',tm' );
        fprintf(1,'tm ct2fl inv \n' );
        fprintf(1,'%16e%16e%16e%16e%16e%16e\n',inv(tm)' );
        tmold = inv(tm);

        %        fprintf(1,'Check the orthogonality (mat*transpose) of tm \n' );
        %        fprintf(1,'%16.12f%16.12f%16.12f%16.12f%16.12f%16.12f\n',(tm * tm')' );

        %        fprintf(1,'Check the xxxx(transpose*mat) of flcov \n' );
        %        fprintf(1,'%16.12f%16.12f%16.12f%16.12f%16.12f%16.12f\n',(flcov' * flcov)' );

        fprintf(1,'----- flight to cartesian covariance \n');
        [cartco,tm] = covfl2ct( flcov,flstate,anomflt, ttt,jdut1,lod,xp,yp,2,ddpsi,ddeps );
        printcov( cartco,'ct','m',anom );

        fprintf(1,'tm  fl2ct \n' );
        fprintf(1,'%16e%16e%16e%16e%16e%16e\n',tm' );

        %        fprintf(1,'Check the orthogonality (mat*transpose) of tm \n' );
        %        fprintf(1,'%16e%16e%16e%16e%16e%16e\n',(tm*tm')' );
        for i=1:6
            for j = 1:6
                if (abs( cartcov(i,j)-cartco(i,j) ) < small) | (abs(cartcov(i,j)) < small)
                    diffmm(i,j) = 0.0;
                else
                    diffmm(i,j) = 100.0*( (cartcov(i,j)-cartco(i,j)) / cartcov(i,j));
                end;
            end;
        end;
        fprintf(1,'-------- accuracy --------- \n');
        fprintf(1,'pct differences if over %4e \n',small);
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',diffmm' );

        for i=1:6
            for j = 1:6
                if (abs( tmold(i,j)-tm(i,j) ) < small) | (abs(tmold(i,j)) < small)
                    diffmm(i,j) = 0.0;
                else
                    diffmm(i,j) = 100.0*( (tmold(i,j)-tm(i,j)) / tmold(i,j));
                end;
            end;
        end;
        fprintf(1,'-------- tm accuracy --------- \n');
        fprintf(1,'pct differences if over %4e \n',small);
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',diffmm' );
        pause;

        fprintf(1,' -------- equinoctial covariance conversions --------- \n');
        fprintf(1,' -----  classical to equinoctial covariance ---------- \n');
        [eqcov,tm] = covcl2eq ( classcov,classstate,anom);
        printcov( eqcov,'eq','m',anom );

        fprintf(1,'tm cl2eq \n' );
        fprintf(1,'%16e%16e%16e%16e%16e%16e\n',tm' );
        fprintf(1,'tm cl2eq inv \n' );
        fprintf(1,'%16e%16e%16e%16e%16e%16e\n',inv(tm)' );
        tmold = inv(tm);

        %        fprintf(1,'Check the orthogonality (mat*transpose) of tm \n' );
        %        fprintf(1,'%16.12f%16.12f%16.12f%16.12f%16.12f%16.12f\n',(tm * tm')' );

        %        fprintf(1,'Check the xxxx(transpose*mat) of eqcov \n' );
        %        fprintf(1,'%16.12f%16.12f%16.12f%16.12f%16.12f%16.12f\n',(eqcov' * eqcov)' );

        fprintf(1,'----- equinoctial to classical covariance \n');
        [classco,tm] = coveq2cl( eqcov,eqstate,anom, fr);
        printcov( classco,'cl','m',anom );

        fprintf(1,'tm eq2cl \n' );
        fprintf(1,'%16e%16e%16e%16e%16e%16e\n',tm' );

        %        fprintf(1,'Check the orthogonality (mat*transpose) of tm \n' );
        %        fprintf(1,'%16e%16e%16e%16e%16e%16e\n',(tm*tm')' );

        for i=1:6
            for j = 1:6
                if (abs( classcov(i,j)-classco(i,j) ) < small) | (abs(classcov(i,j)) < small)
                    diffmm(i,j) = 0.0;
                else
                    diffmm(i,j) = 100.0*( (classcov(i,j)-classco(i,j)) / classcov(i,j));
                end;
            end;
        end;
        fprintf(1,'-------- accuracy --------- \n');
        fprintf(1,'pct differences if over %4e \n',small);
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',diffmm' );

        for i=1:6
            for j = 1:6
                if (abs( tmold(i,j)-tm(i,j) ) < small) | (abs(tmold(i,j)) < small)
                    diffmm(i,j) = 0.0;
                else
                    diffmm(i,j) = 100.0*( (tmold(i,j)-tm(i,j)) / tmold(i,j));
                end;
            end;
        end;
        fprintf(1,'-------- accuracy tm --------- \n');
        fprintf(1,'pct differences if over %4e \n',small);
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',diffmm' );



        pause;


        % ------------------------ check nav tests -----------------------------------
        small = 1.0e-18;
        doall = 'n';
        %        fprintf(1,'year mon day hms  magr  magv  r sig  vsig  max diag  max mat \n');
        fprintf(1,'      ecc         incl        maxdiag        maxdiff        magr        r sig   \n');
        for testnumbb = 1:12  % total is 122
            %        for testnumb = 1:122
            if testnumbb == 1
                testnumb = 3;
                satnum =   107;
            end;
            if testnumbb == 2
                testnumb = 16;
                satnum =    11;
            end;
            if testnumbb == 3
                testnumb = 28;
                satnum = 16609;
            end;
            if testnumbb == 4
                testnumb = 42;
                satnum = 20052;
            end;
            if testnumbb == 5
                testnumb = 52;
                satnum = 21867;
            end;
            if testnumbb == 6
                testnumb = 64;
                satnum = 23019;
            end;
            if testnumbb == 7
                testnumb = 76;
                satnum = 24780;
            end;
            if testnumbb == 8
                testnumb = 87;
                satnum = 25013;
            end;
            if testnumbb == 9
                testnumb = 97;
                satnum = 25544;
            end;
            if testnumbb == 10
                testnumb = 101;
                satnum = 25634;
            end;
            if testnumbb == 11
                testnumb = 110;
                satnum = 26354;
            end;
            if testnumbb == 12
                testnumb = 113;
                satnum = 26405;
            end;
            testcove;


            fprintf(1,'year %5i ',year);
            fprintf(1,' mon %4i ',mon);
            fprintf(1,' day %3i ',day);
            fprintf(1,'%3i:%2i:%8.6f\n ',hr,min,sec );
            fprintf(1,'dut1 %8.6f s',dut1);
            fprintf(1,' dat %3i s',dat);
            fprintf(1,' xp %8.6f "',xp);
            fprintf(1,' yp %8.6f "',yp);
            fprintf(1,' lod %8.6f s\n',lod);
            fprintf(1,' ddpsi %8.6f " ddeps  %8.6f\n',ddpsi,ddeps);
            %        fprintf(1,'order %3i  eqeterms %31  opt %3s \n',order,eqeterms,opt );
            fprintf(1,'units are km and km/s and km/s2\n' );
            fprintf(1,'eci%14.7f%14.7f%14.7f',reci );
            fprintf(1,' v %14.9f%14.9f%14.9f\n',veci );

            [ut1, tut1, jdut1, jdut1frac, utc, tai, tt, ttt, jdtt, jdttfrac, tdb, ttdb, jdtdb, jdtdbfrac, tcg, jdtcg,jdtcgfrac, tcb, jdtcb,jdtcbfrac ] ...
                = convtime ( year, mon, day, hr, min, sec, timezone, dut1, dat );

            fprintf(1,'ut1 %8.6f tut1 %16.12f jdut1 %18.11f\n',ut1,tut1,jdut1 );
            fprintf(1,'utc %8.6f\n',utc );
            fprintf(1,'tai %8.6f\n',tai );
            fprintf(1,'tt  %8.6f ttt  %16.12f jdtt  %18.11f\n',tt,ttt,jdtt );
            fprintf(1,'tdb %8.6f ttdb %16.12f jdtdb %18.11f\n',tdb,ttdb,jdtdb );

            [p,a,ecc,incl,omega,argp,nu,m,arglat,truelon,lonper ] = rv2coe (reci,veci);
            fprintf(1,'          p km       a km      ecc      incl deg     raan deg     argp deg      nu deg      m deg      arglat   truelon    lonper\n');
            fprintf(1,'coes %11.4f%11.4f%13.9f%13.7f%14.8f%14.8f%14.8f%14.8f%14.8f%14.8f%14.8f\n',...
                p,a,ecc,incl*rad,omega*rad,argp*rad,nu*rad,m*rad, ...
                arglat*rad,truelon*rad,lonper*rad );


            [cartstate,classstate,flstate,eqstate] = setcov(reci,veci, ...
                year,mon,day,hr,min,sec,dut1,dat,ttt,jdut1,lod,xp,yp,terms,'n',anom,ddpsi,ddeps);




            fprintf(1,'new case --------------\n');

            for i=1:6
                eqcov(i,4) = eqcov(i,4)/tusec;
            end;
            for i=1:6
                eqcov(4,i) = eqcov(4,i)/tusec;
            end;

            % --------------------- write out input data --------------------------
            if doall ~= 'y'
                %                printcov( eqcov,'eq','m',anom );
            end;
            printcov( eqcov,'eq','m',anom );

            % ------ do conversions
            [classcov,tm] = coveq2cl( eqcov,eqstate,anom, fr);
            printcov( classcov,'cl','m',anom );

            [cartcov,tm] = covcl2ct( classcov,classstate,anom );
            printcov( cartcov,'ct','m',anom );


            [classco,tm] = covct2cl(cartcov,cartstate,anom );
            printcov( classco,'cl','m',anom );

            [eqcovmeannrev,tm] = covcl2eq ( classco,classstate,anom);
            if doall ~= 'y'
                %                printcov( eqco,'eq','m',anom );
            end;
            printcov( eqcovmeannrev,'eq','m',anom );

            for i=1:6
                for j = 1:6
                    if (abs( eqcov(i,j)-eqcovmeannrev(i,j) ) < small) | (abs(eqcov(i,j)) < small)
                        diffmm(i,j) = 0.0;
                    else
                        diffmm(i,j) = 100.0*( (eqcov(i,j)-eqcovmeannrev(i,j)) / eqcov(i,j));
                    end;
                end;
            end;
            if doall ~= 'y'
                %                fprintf(1,'accuracy \n');
                %                fprintf(1,'%11.4f%11.4f%11.4f%11.4f%11.4f%11.4f\n',diffm' );
            end;
            %            fprintf(1,'accuracy \n');
            %            fprintf(1,'%11.4f%11.4f%11.4f%11.4f%11.4f%11.4f\n',diffm' );

            magr = sqrt( cartcov(1,1) + cartcov(2,2) + cartcov(3,3) );
            magv = sqrt( cartcov(4,4) + cartcov(5,5) + cartcov(6,6) );

            magrs = sqrt( possigma(1)^2 + possigma(2)^2 + possigma(3)^2 )*1000.0;
            magvs = sqrt( velsigma(1)^2 + velsigma(2)^2 + velsigma(3)^2 )*1000.0;

            if doall ~= 'y'
                fprintf(1,'position mag %11.5f velocity %11.5f \n',magr,magv );
                fprintf(1,'position sig from msg %11.5f velocity %11.5f \n',magrs,magvs );
                fprintf(1,'-------------------------------------------------\n' );
            end;

            md = max(diag(abs(diffmm)));
            mm = max(max(abs(diffmm)));

            if doall ~= 'y'
                %                fprintf(1,'%3i %5i %3i %3i %3i:%2i:%8.6f %11.5f%11.5f%11.5f%11.5f%11.5f%11.5f\n', ...
                %                          testnumb,year,mon,day,hr,min,sec,magr,magv,magrs,magvs,md,mm );
                %                if ( md > 0.1 ) | ( mm > 0.1 )
                %                    fprintf(1,'input equinoctial covariance \n');
                %                    printcov( eqcov,'eq','m',anom );
                %                    fprintf(1,'calculated equinoctial covariance \n');
                %                    printcov( eqco,'eq','m',anom );
                %                  end
            end;
            fprintf(1,'%11.6f   %11.6f   %14.6f %14.6f %14.6f %11.6f %3i \n', ...
                classstate(2),classstate(3)*rad,md,mm,magr,magrs,testnumb );

        end;


        %Consolidated verifcation
        small = 1.0e-18;
        cartcov = [ ...
            100.0,  1.0e-2, 1.0e-2, 1.0e-4,   1.0e-4,   1.0e-4; ...
            1.0e-2, 100.0,  1.0e-2, 1.0e-4,   1.0e-4,   1.0e-4; ...
            1.0e-2, 1.0e-2, 100.0,  1.0e-4,   1.0e-4,   1.0e-4; ...
            1.0e-4, 1.0e-4, 1.0e-4, 0.0001,   1.0e-6,   1.0e-6; ...
            1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6,   0.0001,   1.0e-6; ...
            1.0e-4, 1.0e-4, 1.0e-4, 1.0e-6,   1.0e-6,   0.0001];

        % ct2cl meana
        classcov =    [ 7.2998470527E+02    3.2622542748E-05    1.9882701518E-07   -1.5267354781E-07    6.5719274495E-02   -6.5719721795E-02
            3.2622542748E-05    4.7913185965E-12    1.6984420631E-14   -1.3041848226E-14    1.9093724426E-09   -1.9144294314E-09
            1.9882701518E-07    1.6984420631E-14    1.8210304470E-12    1.8574598946E-13    2.2175774749E-11   -2.2160515490E-11
            -1.5267354781E-07   -1.3041848226E-14    1.8574598946E-13    2.0516270435E-12   -1.6735967689E-11    1.7016422633E-11
            6.5719274495E-02    1.9093724426E-09    2.2175774749E-11   -1.6735967689E-11    7.2061287418E-06   -7.2039903813E-06
            -6.5719721795E-02   -1.9144294314E-09   -2.2160515490E-11    1.7016422633E-11   -7.2039903813E-06    7.2018610163E-06];

        tmct2clm = [   -1.7683319064E-01   -1.7135436636E+00    1.0196363131E+00   -3.7038514352E+02   -8.7440859633E+02   -1.5303032219E+03
            -3.2616946703E-08   -1.1660253726E-07   -8.1284934173E-08   -3.3159300134E-05   -1.5533153893E-04   -3.2954249110E-05
            7.3161070330E-08   -1.3497524690E-08   -9.9950269315E-09   -1.0964037725E-04    2.0227611381E-05    1.4978710924E-05
            1.2294251173E-07   -2.2681729232E-08   -1.6796005174E-08    6.6277613650E-05   -1.2227592114E-05   -9.0546315192E-06
            8.6134774380E-07   -7.8604704844E-05    1.1232839062E-04   -4.1439495269E-02   -6.4242797520E-02   -2.1663961239E-01
            -8.1498812514E-07    7.8672438414E-05   -1.1220665948E-04    4.1471465416E-02    6.4465640062E-02    2.1650469223E-01];


        cartcov1 = [    1.0000000044E+02    9.9479437413E-03    1.0077060115E-02    9.9969642671E-05    9.9952965602E-05    9.9850351635E-05
            9.9479482267E-03    9.9999494542E+01    1.0306186086E-02    9.9911319273E-05    9.9975417086E-05    9.9435118941E-05
            1.0077058270E-02    1.0306163211E-02    1.0000015130E+02    9.9894711415E-05    9.9685411584E-05    9.9664977000E-05
            9.9969640504E-05    9.9911316301E-05    9.9894701574E-05    1.0000005983E-04    1.0001511979E-06    1.0002207062E-06
            9.9952963204E-05    9.9975432157E-05    9.9685393138E-05    1.0001511890E-06    1.0000032120E-04    1.0006353763E-06
            9.9850357726E-05    9.9435120969E-05    9.9665007269E-05    1.0002207092E-06    1.0006353547E-06    1.0000073117E-04];

        tmcl2ctm = [   -8.8298081674E-02   -2.3062066390E+06    3.4350827677E+06    5.8702295255E+06   -1.4097366000E+06   -1.4115824070E+06
            -8.5562340421E-01   -3.5744128790E+06   -6.3374024247E+05   -6.0579222533E+05   -3.3238318480E+06   -3.3324764960E+06
            5.0913478173E-01   -1.2053876478E+07   -4.6928980979E+05    0.0000000000E+00   -5.8303330764E+06   -5.8321699765E+06
            1.1429153396E-04   -4.7014983029E+01   -6.3719510650E+03    3.7023488924E+03    6.7211739544E+02    6.7393260795E+02
            2.6982048565E-04    4.3734397244E+03    1.1755646330E+03   -1.5682542900E+03    6.5240298553E+03    6.5305214031E+03
            4.7221306357E-04   -6.2501359462E+03    8.7051518276E+02    0.0000000000E+00   -3.8904773582E+03   -3.8859568039E+03];
        fprintf(1,'diff cartcov - cartcov1 \n');
        fprintf(1,'%20.10e%20.10e%20.10e%20.10e%20.10e%20.10e\n',cartcov' - cartcov1');
        fprintf(1,'pctdiff cartcov - cartcov1 \n');
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',100.0*((cartcov' - cartcov1')/cartcov'));

        fprintf(1,'diff tmct2cl - inv(tmcl2ct) \n');
        fprintf(1,'%20.10e%20.10e%20.10e%20.10e%20.10e%20.10e\n',tmct2clm' - inv(tmcl2ctm)');
        fprintf(1,'pctdiff tmct2cl - inv(tmcl2ct) \n');
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',100.0*((tmct2clm' - inv(tmcl2ctm)')/tmct2clm'));

        fprintf(1,'diff tmcl2ct - inv(tmct2cl) \n');
        fprintf(1,'%20.10e%20.10e%20.10e%20.10e%20.10e%20.10e\n',tmcl2ctm' - inv(tmct2clm)');
        fprintf(1,'pctdiff tmcl2ct - inv(tmct2cl) \n');
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',100.0*((tmcl2ctm' - inv(tmct2clm)')/tmcl2ctm'));

        % ct2cl truea
        classcov = [    7.2998470527E+02    3.2622542748E-05    1.9882701518E-07   -1.5267354781E-07    6.5719274495E-02   -6.5719230531E-02
            3.2622542748E-05    4.7913185965E-12    1.6984420631E-14   -1.3041848226E-14    1.9093724426E-09   -1.9074392492E-09
            1.9882701518E-07    1.6984420631E-14    1.8210304470E-12    1.8574598946E-13    2.2175774749E-11   -2.2149481645E-11
            -1.5267354781E-07   -1.3041848226E-14    1.8574598946E-13    2.0516270435E-12   -1.6735967689E-11    1.7007950060E-11
            6.5719274495E-02    1.9093724426E-09    2.2175774749E-11   -1.6735967689E-11    7.2061287418E-06   -7.2069633456E-06
            -6.5719230531E-02   -1.9074392492E-09   -2.2149481645E-11    1.7007950060E-11   -7.2069633456E-06    7.2078001121E-06
            ];

        tmct2clt = [  -1.7683319064E-01   -1.7135436636E+00    1.0196363131E+00   -3.7038514352E+02   -8.7440859633E+02   -1.5303032219E+03
            -3.2616946703E-08   -1.1660253726E-07   -8.1284934173E-08   -3.3159300134E-05   -1.5533153893E-04   -3.2954249110E-05
            7.3161070330E-08   -1.3497524690E-08   -9.9950269315E-09   -1.0964037725E-04    2.0227611381E-05    1.4978710924E-05
            1.2294251173E-07   -2.2681729232E-08   -1.6796005174E-08    6.6277613650E-05   -1.2227592114E-05   -9.0546315192E-06
            8.6134774380E-07   -7.8604704844E-05    1.1232839062E-04   -4.1439495269E-02   -6.4242797520E-02   -2.1663961239E-01
            -8.7495409985E-07    7.8531006978E-05   -1.1245460285E-04    4.1448320342E-02    6.4241169378E-02    2.1663840674E-01
            ];

        cartcov1 = [     9.9999999878E+01    9.9838874085E-03    1.0023672447E-02    9.9990547615E-05    9.9985265964E-05    9.9951263568E-05
            9.9838796334E-03    9.9999667381E+01    1.0334126229E-02    9.9942380210E-05    9.9878108156E-05    9.9776738608E-05
            1.0023654480E-02    1.0334115948E-02    9.9999719738E+01    1.0000501669E-04    1.0005691598E-04    9.9940448393E-05
            9.9990549496E-05    9.9942380941E-05    1.0000501588E-04    1.0000004013E-04    1.0000408179E-06    1.0002290301E-06
            9.9985272285E-05    9.9878087092E-05    1.0005685897E-04    1.0000408235E-06    1.0000003267E-04    1.0002512404E-06
            9.9951259431E-05    9.9776750265E-05    9.9940480844E-05    1.0002290269E-06    1.0002512422E-06    1.0000129048E-04
            ];

        tmcl2ctt = [   -8.8298080209E-02    2.5515193503E+05    3.4350828085E+06    5.8702295018E+06   -1.4097366043E+06   -1.4103215189E+06
            -8.5562340076E-01    2.4724656056E+06   -6.3374025000E+05   -6.0579221528E+05   -3.3238318898E+06   -3.3294998116E+06
            5.0913478778E-01   -1.4712293403E+06   -4.6928981536E+05    0.0000000000E+00   -5.8303330515E+06   -5.8269603806E+06
            1.1429153431E-04   -1.2698858940E+03   -6.3719510378E+03    3.7023489388E+03    6.7211738428E+02    6.7333060821E+02
            2.6982048904E-04   -7.4763870430E+03    1.1755646279E+03   -1.5682542948E+03    6.5240298289E+03    6.5246880053E+03
            4.7221306155E-04    8.0104911367E+02    8.7051517904E+02    0.0000000000E+00   -3.8904774044E+03   -3.8824857291E+03
            ];

        fprintf(1,'diff cartcov - cartcov1 \n');
        fprintf(1,'%20.10e%20.10e%20.10e%20.10e%20.10e%20.10e\n',cartcov' - cartcov1');
        fprintf(1,'pctdiff cartcov - cartcov1 \n');
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',100.0*((cartcov' - cartcov1')/cartcov'));

        fprintf(1,'diff tmct2cl - inv(tmcl2ct) \n');
        fprintf(1,'%20.10e%20.10e%20.10e%20.10e%20.10e%20.10e\n',tmct2clt' - inv(tmcl2ctt)');
        fprintf(1,'pctdiff tmct2cl - inv(tmcl2ct) \n');
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',100.0*((tmct2clt' - inv(tmcl2ctt)')/tmct2clt'));

        fprintf(1,'diff tmcl2ct - inv(tmct2cl) \n');
        fprintf(1,'%20.10e%20.10e%20.10e%20.10e%20.10e%20.10e\n',tmcl2ctt' - inv(tmct2clt)');
        fprintf(1,'pctdiff tmcl2ct - inv(tmct2cl) \n');
        fprintf(1,'%14.4f%14.4f%14.4f%14.4f%14.4f%14.4f\n',100.0*((tmcl2ctt' - inv(tmct2clt)')/tmcl2ctt'));


        % lance test



        eqCovmeana = [
            7.2998470594E+02   -5.1232022015E-05   -5.7697021750E-05    1.9389174770E-07    2.1327374042E-07   -5.9997332907E-07
            -5.1232022015E-05    6.1778006946E-12    2.6215354700E-12   -2.2655470832E-14   -2.3898387998E-14    4.2135772123E-12
            -5.7697021750E-05    2.6215354700E-12    6.7713618731E-12   -1.8031914124E-14   -1.6405273668E-14   -3.6135835764E-12
            1.9389174770E-07   -2.2655470832E-14   -1.8031914124E-14    2.5193665805E-12   -2.7497822431E-13    7.1178436810E-13
            2.1327374042E-07   -2.3898387998E-14   -1.6405273668E-14   -2.7497822431E-13    2.5859797681E-12   -2.5800265285E-12
            -5.9997332907E-07    4.2135772123E-12   -3.6135835764E-12    7.1178436810E-13   -2.5800265285E-12    1.1608008196E-11];
        svd(eqCovmeana);
        s1eqA0 = sqrt(diag(svd(eqCovmeana)));

        eqCovmeann = [
            4.3069707203E-02    3.9352341277E-07    4.4318236940E-07   -1.4893213124E-09   -1.6381982769E-09    4.6085151970E-09
            3.9352341277E-07    6.1778006946E-12    2.6215354700E-12   -2.2655470832E-14   -2.3898387998E-14    4.2135772123E-12
            4.4318236940E-07    2.6215354700E-12    6.7713618731E-12   -1.8031914124E-14   -1.6405273668E-14   -3.6135835764E-12
            -1.4893213124E-09   -2.2655470832E-14   -1.8031914124E-14    2.5193665805E-12   -2.7497822431E-13    7.1178436810E-13
            -1.6381982769E-09   -2.3898387998E-14   -1.6405273668E-14   -2.7497822431E-13    2.5859797681E-12   -2.5800265285E-12
            4.6085151971E-09    4.2135772123E-12   -3.6135835764E-12    7.1178436810E-13   -2.5800265285E-12    1.1608008196E-11];

        svd(eqCovmeann);
        s1eqN0 = sqrt(diag(svd(eqCovmeann)));

        a=[s1eqA0(1,1) s1eqN0(1,1)
            s1eqA0(2,2) s1eqN0(2,2)
            s1eqA0(3,3) s1eqN0(3,3)
            s1eqA0(4,4) s1eqN0(4,4)
            s1eqA0(5,5) s1eqN0(5,5)
            s1eqA0(6,6) s1eqN0(6,6)]




        % test sigmapts -------------------------------------------
        cov3(1, 1) = 12559.93762571587;
        cov3(1, 2) = 12101.56371305036;
        cov3(1, 3) = -440.3145384949657;
        cov3(2, 1) = 12101.56371305036;
        cov3(2, 2) = 12017.77368889201;
        cov3(2, 3) = 270.3798093532698;
        cov3(3, 1) = -440.3145384949657;
        cov3(3, 2) = 270.3798093532698;
        cov3(3, 3) = 4818.009967057008;

        r1(1) = -2.85781791674123e+007; % all m and m/s here
        r1(2) =  3.09956160912026e+007;
        r1(3) = -2.26122833628375e+003;
        v1(1) = 3.37547320575159e+004;
        v1(2) = -2.08388260114718e+003;
        v1(3) = 5.18744171831942e-001;

        r1(1) =    -30762454.8061775;
        r1(2) =    -28804817.6521682;
        r1(3) =    -991451.166480117;

        fprintf(1,'cov3 starting \n');
        %    fprintf(1,'%20.10e %20.10e %20.10e %20.10e %20.10e %20.10e\n',cov3);
        fprintf(1,' %20.10e %20.10e %20.10e\n',cov3);

        eig(cov3);

        [eigenaxes,d] = eig(cov3);
        eigenvalues = sqrt(d);  % in m
        %eigenvalues
        %     fprintf(1,'eigenaxes  %16.8f  %16.8f  %16.8f  %16.8f  %16.8f  %16.8f  m \n', eigenaxes(1:6) );
        %     fprintf(1,'eigenaxes  %16.8f  %16.8f  %16.8f  %16.8f  %16.8f  %16.8f  m \n', eigenaxes(7:12) );
        %     fprintf(1,'eigenaxes  %16.8f  %16.8f  %16.8f  %16.8f  %16.8f  %16.8f  m \n', eigenaxes(13:18) );
        %     fprintf(1,'eigenaxes  %16.8f  %16.8f  %16.8f  %16.8f  %16.8f  %16.8f  m \n', eigenaxes(19:24) );
        %     fprintf(1,'eigenaxes  %16.8f  %16.8f  %16.8f  %16.8f  %16.8f  %16.8f  m \n', eigenaxes(25:30) );
        %     fprintf(1,'eigenaxes  %16.8f  %16.8f  %16.8f  %16.8f  %16.8f  %16.8f  m \n', eigenaxes(31:36) );
        %     fprintf(1,'eigenvalues   %16.8f  %16.8f  %16.8f  %16.8f  %16.8f  %16.8f  m \n', eigenvalues(1,1), eigenvalues(2,2), eigenvalues(3,3), ...
        %                eigenvalues(4,4), eigenvalues(5,5), eigenvalues(6,6) );
        %     fprintf(1,'mag eigenvalues  %16.8f m  \n', sqrt(eigenvalues(1,1)^2+ eigenvalues(2,2)^2+ eigenvalues(3,3)^2)  );
        %     fprintf(1,'mag eigenvalues  %16.8f m/s \n', sqrt(eigenvalues(4,4)^2+ eigenvalues(5,5)^2+ eigenvalues(6,6)^2)  );

        % ----- setup 12 initial states

        % ---- find sigma points
        sigmapts = poscov2pts(r1, cov3);
        sigmapts

        [yu,covout] = remakecov(sigmapts);
        covout

        % test pos/vel
        r1(1) =    -30762454.8061775;
        r1(2) =    -28804817.6521682;
        r1(3) =    -991451.166480117;
        v1(1) = 2102.69117282211;
        v1(2) = -2239.85220168736;
        v1(3) = -140.227078892292;

        cov2(1, 1) = 12559.93762571587;
        cov2(1, 2) = 12101.56371305036;
        cov2(1, 3) = -440.3145384949657;
        cov2(1, 4) = -0.8507401236198346;
        cov2(1, 5) = 0.9383675791981778;
        cov2(1, 6) = -0.0318596430999798;
        cov2(2, 2) = 12017.77368889201;
        cov2(2, 3) = 270.3798093532698;
        cov2(2, 4) = -0.8239662300032132;
        cov2(2, 5) = 0.9321640899868708;
        cov2(2, 6) = -0.001327326827629336;
        cov2(3, 3) = 4818.009967057008;
        cov2(3, 4) = 0.02033418761460195;
        cov2(3, 5) = 0.03077663516695039;
        cov2(3, 6) = 0.1977541628188323;
        cov2(4, 4) = 5.774758755889862e-005;
        cov2(4, 5) = -6.396031584925255e-005;
        cov2(4, 6) = 1.079960679599204e-006;
        cov2(5, 5) = 7.24599391355188e-005;
        cov2(5, 6) = 1.03146660433274e-006;
        cov2(6, 6) = 1.870413627417302e-005;
        cov2(2, 1) = 12101.56371305036;
        cov2(3, 1) = -440.3145384949657;
        cov2(4, 1) = -0.8507401236198346;
        cov2(5, 1) = 0.9383675791981778;
        cov2(6, 1) = -0.0318596430999798;
        cov2(3, 2) = 270.3798093532698;
        cov2(4, 2) = -0.8239662300032132;
        cov2(5, 2) = 0.9321640899868708;
        cov2(6, 2) = -0.001327326827629336;
        cov2(4, 3) = 0.02033418761460195;
        cov2(5, 3) = 0.03077663516695039;
        cov2(6, 3) = 0.1977541628188323;
        cov2(5, 4) = -6.396031584925255e-005;
        cov2(6, 4) = 1.079960679599204e-006;
        cov2(6, 5) = 1.03146660433274e-006;

        sigmapts = posvelcov2pts(r1, v1, cov2);

        [yu,covout] = remakecov(sigmapts);
        covout




