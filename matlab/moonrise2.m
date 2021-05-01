% -----------------------------------------------------------------------------
%
%                           function moonriset
%
%  this function finds the universal time for moonrise and moonset given the
%    day and site location. notice this routine kicks out if the event
%    occurs on the next or previous day. 
%
%  author        : david vallado                  719-573-2600    1 mar 2001
%
%  revisions
%                - david vallado                                 25 jan 2011
%
%  inputs          description                    range / units
%    jd          - julian date                    days from 4713 bc
%    latgd       - site latitude (south -)        -65ø to 65ø rad
%    lon         - site longitude (west -)        -2pi to 2pi rad
%
%  outputs       :
%    utmoonrise  - universal time of moonrise     hrs
%    utmoonset   - universal time of moonset      hrs
%    moonphaseang- phase angle of the moon        deg
%    error       - error parameter
%
%  locals        :
%    moonangle   - angle between the moon vector
%                  and a point on the earth       rad
%    jdtemp      - julian date for moonrise/set   days from 4713 bc
%    uttemp      - temporary ut time              days
%    tut1        - julian centuries from the
%                  jan 1, 2000 12 h epoch (ut1)
%    rtasc       - right ascension                rad
%    decl        - declination                    rad
%    meanlonmoon -                                rad
%    meananomaly -                                rad
%    eclplong    - longitude of the ecliptic      rad
%    obliquity   - obliquity of the ecliptic      rad
%    rmoon
%    rmoonrs
%    rv
%    rhosat
%    tsry1
%    l, m, n     - direction cosines
%    eclplat
%    moongha, moonghan
%    dgha, dghan
%    lhan
%    lst
%    deltaut, deltautn
%    t, tn
%    hzparal
%    loneclsun
%    loneclmoon
%    ttdb
%    gst         - for 0 h utc of each day        rad
%    lha         - local hour angle               rad
%    year        - year                           1900 .. 2100
%    mon         - month                          1 .. 12
%    day         - day                            1 .. 28,29,30,31
%    hr          - hour                           0 .. 23
%    min         - minute                         0 .. 59
%    sec         - second                         0.0  .. 59.999
%    opt         - idx to for rise and set calc    1,2
%
%  coupling      :
%    invjulian- finds the year day mon hr min sec from the julian date
%    julianday   - finds the julian date given year, mon day, hr, min, sec
%
%  references    :
%    vallado       2007, 292, Alg 32, Ex 5-4
%
% [utmoonrise,utmoonset,moonphaseang,error] = moonrise( jd,latgd,lon )
% -----------------------------------------------------------------------------

function [utmoonrise,utmoonset,moonphaseang,error] = moonrise2( jd,latgd,lon, show )
    twopi    =  2.0*pi;
    deg2rad  =  pi/180.0;

    % ------------------------  implementation   ------------------
    error    = 'ok';

    % -------------- for once for moonrise (1), ) set (2) ---------
    % -------------- make sure lon is within +- 180 deg -----------
    if ( lon > pi )
        lon = lon - 2.0 * pi;
    end
    if ( lon < -pi )
        lon = lon + twopi;
    end

    try1 = 1; % try another approach on the current option
    opt  = 1; % 1-2 for rise/set
    while (opt <= 2)
        [year,month,day,hr,min,sec] = invjday(jd,0.0);
        [jdtemp,jdtempf] = jday(year,month,day,0,0,0.0);

        if ( opt == 1 )  % rise
            uttemp = (6.0 - lon/15.0)/24.0;  % 20 and 4
        else             % set
            uttemp = (18.0 + lon/15.0)/24.0;
        end

        if ( try1 == 2 ) % only set if a problem
            uttemp = 0.5;
        end

        i  = 0;
        tn = uttemp;
        t  = tn + 10.0;
        jdtemp = jdtemp+jdtempf + uttemp;
        deltaut = 10.0;
        ktlim = 15;  % 6

        if show == 'y'
            fprintf('   ecplon    ecllat       l          m           n       rtasc          decl        gmst       mgha      dgha    lhan     lhan1   deltaut     tn    deltaut    jd    tn \n' );
        end;
        
        
        while ( (abs(deltaut) >= 0.00008 ) && (i < ktlim) )
            ttdb = ( jdtemp+jdtempf - 2451545.0 ) / 36525.0;
            eclplong = 218.32 + 481267.8813 * ttdb  ...
                + 6.29 * sin( (134.9 + 477198.85 * ttdb) * deg2rad )  ...
                - 1.27 * sin( (259.2 - 413335.38 * ttdb) * deg2rad )  ...
                + 0.66 * sin( (235.7 + 890534.23 * ttdb) * deg2rad )  ...
                + 0.21 * sin( (269.9 + 954397.70 * ttdb) * deg2rad )  ...
                - 0.19 * sin( (357.5 + 35999.05 * ttdb) * deg2rad )   ...
                - 0.11 * sin( (186.6 + 966404.05 * ttdb) * deg2rad );
            eclplat = 5.13 * sin( ( 93.3 + 483202.03 * ttdb) * deg2rad )  ...
                + 0.28 * sin( (228.2 + 960400.87 * ttdb) * deg2rad ) ...
                - 0.28 * sin( (318.3 +  6003.18 * ttdb) * deg2rad )  ...
                - 0.17 * sin( (217.6 - 407332.20 * ttdb) * deg2rad );
            eclplong = rem( eclplong * deg2rad, twopi );
            eclplat  = rem( eclplat * deg2rad, twopi );
            if show == 'y'
                fprintf('%2i %2i %11.7f %11.7f ',opt, i,eclplong/deg2rad, eclplat/deg2rad);
            end;
            obliquity = 23.439291 - 0.0130042 * ttdb;
            obliquity = obliquity * deg2rad;
            
            % ------- find the geocentric direction cosines -------
            l = cos( eclplat ) * cos( eclplong );
            m = cos(obliquity) * cos(eclplat) * sin(eclplong) ...
                - sin(obliquity) * sin(eclplat);
            n = sin(obliquity) * cos(eclplat) *  sin(eclplong) ...
                + cos(obliquity) * sin(eclplat);
            if show == 'y'
                fprintf(' %11.7f %11.7f %11.7f ',l, m, n);
            end;
            rtasc = atan2( m,l );
            % - check that rtasc is in the same quadrant as eclplong
            %        if ( eclplong < 0.0  )
            %            eclplong = eclplong + twopi;
            %        end
            if ( abs( eclplong - rtasc )  >  pi*0.5  )
                rtasc = rtasc + 0.5 * pi * fix( 0.5 + (eclplong - rtasc) / (0.5 *pi) );
            end
            decl = asin( n );
            
            [lst,gmst] = lstime(lon,jdtemp);
            if show == 'y'
                fprintf(' %d %d %d ',rtasc, decl, gmst/deg2rad);
            end;
            moonghan= gmst - rtasc;

            %cdav
            lha  = moonghan + lon;

 %           % find elevation directly at this time
 %           el = asin( sin(latgd) * sin(rtasc) + cos(latgd) * cos(rtasc) * cos(lha) );
            
            if ( i  ==  0 )
                dgha = 347.81  * deg2rad;
            else
                dgha = (moonghan - moongha) / deltaut;
            end
            %        if ( dgha < 0.0  )
            %            dgha = dgha + twopi / abs(deltaut);
            %        end
            if show == 'y'
                fprintf(' %11.7f  %11.7f ',moonghan/deg2rad, dgha/deg2rad);
            end;

            lhan = 0.00233 - (sin(latgd) * sin(decl)) / (cos(latgd)* cos(decl));
            if show == 'y'
                fprintf(' %11.7f rad ',lhan);
            end;
            %fprintf('lhan  %11.7f deg \n',lhan/deg2rad);
            %       if ( lhan > 1.0  )
            %           lhan = 0.0;
            %       end
            %       if ( lhan < -1.0  )
            %           lhan = -1.0;
            %       end


            if abs(lhan) > 1.0  % new if
                deltaut = 1.0; % advance to the next day, yes
                uttemp = 9999.9;  % day after
                i = ktlim; % end this trial
              else
                lhan = acos( lhan );
                if ( opt == 1 )
                    lhan = twopi - lhan;
                end
                if show == 'y'
                    fprintf(' %11.7f ',lhan/deg2rad);
                end;
                deltaut = (lhan - lha ) / dgha;
                if show == 'y'
                    fprintf(' % 11.7f  %11.7f ',deltaut, tn);
                end;
%                if ( abs( deltaut ) > 0.5 )
%                    if ( abs( dgha ) > 0.001 )
                        if ( deltaut < -0.5 ) % event is day before no effect
                            deltaut = deltaut + twopi / dgha;
%                            if ( abs( deltaut ) > 0.51 )
%                                uttemp = -9999.9;  % day before
%                                i = ktlim; % end this trial
%                            end
                        else
                            if ( deltaut > 0.5 ) % event is day before
                                deltaut = deltaut - twopi / dgha;
%                                if ( abs( deltaut ) > 0.51 )
%                                    i = ktlim; % end this trial needed
%                                    uttemp = 9999.9;
%                                end
                            end    
                        end
                        
%                    else
%                        error = 'error2 dgha is too small';
%                        fprintf('dgha too small %11.7f ',dgha);
%                    end
%                end

%                 if ( abs( deltaut ) > 0.5 )
%                     if ( abs( dgha ) > 0.001 )
%                         if ( deltaut < 0.0 ) % event is day before
%                             deltaut = deltaut + twopi / dgha;
%                             if ( abs( deltaut ) > 0.51 )
%                                 uttemp = -9999.9;  % day before
%                                 i = ktlim; % end this trial
%                             end
%                         else
%                             deltaut = deltaut - twopi / dgha;
%                             if ( abs( deltaut ) > 0.51 )
%                                 i = ktlim; % end this trial needed
%                                 uttemp = 9999.9;
%                             end
%                         end
%                     else
%                         error = 'error2 dgha is too small';
%                         fprintf('dgha too small %11.7f ',dgha);
% 
%                     end
%                 end

                if uttemp + deltaut < 0.0
                    deltaut = deltaut - 1.0; % advance, go back 1 day 
                    i = ktlim; % end this trial
                    uttemp = -9999.9;
                end;

                i = i + 1;
                moongha = moonghan;

            end;   % new if
            % change tn-t to uttemp for iteration towards 0.0

            jdtemp = jdtemp+jdtempf + deltaut; %
            uttemp = uttemp + deltaut;
            tn = uttemp;
            if show == 'y'
                fprintf(' % 11.7f  %14.4f  %11.7f \n',deltaut, jdtemp+jdtempf, tn);
            end;
        end  % while( (abs(tn-t) >= 0.008 ) && (i <= ktlim) )

        uttemp = tn * 24.0;  % hrs
        if ( tn > 2.0 )  % end the trials as it's going off the end
            i = ktlim;
            uttemp = 9999.99;
        end

%if ( uttemp > 24.0) && (uttemp < 9999.0 )
            % fprintf('rem %11.7f ',uttemp);

%  uttemp = rem( uttemp,24.0 );
            % fprintf('%11.7f ',uttemp);
%end
%        if ( uttemp < 0.0 )
%            uttemp = uttemp + 24.0;
%        end
        %    if ( uttemp > 900 )
        %        uttemp = 24.0;
        %        fprintf('this cannot happen\n');
        %    end

        if ( opt == 1 )
            utmoonrise = uttemp;
        end
        if ( opt == 2 )
            utmoonset = uttemp;
        end

        % update the iteration and check for solution
        try1 = try1 + 1;
        if ( (i  >  ktlim-1) && (try1 < 3) )
            if show == 'y'
                fprintf('try1 #2 %4d \n',opt);
                fprintf(' uttemp  %11.7f  hrs \n', uttemp);
            end;
        else
%             if ( (i > ktlim-1) && (try1 > 2) )
%                 if ( opt == 1 )
%                     error = 'no rise';
%                     uttemp = 9999.99;
%                 end
%                 if ( opt == 2 )
%                     error = 'no set';
%                     uttemp = 9997.99;
%                 end
%             end
            if show == 'y'
                fprintf(' uttemp  %11.7f  hrs \n', uttemp);
            end;
            opt = opt + 1;
            try1 = 1;
        end


    end % while opt <= 2

    
    
    % ------------- determine phase angle of the moon -------------
    meanlong = 280.4606184 + 36000.77005361 * ttdb;
    meanlong = rem( meanlong,360.0 );

    meananomaly = 357.5277233 + 35999.05034 * ttdb;
    meananomaly = rem( meananomaly*deg2rad,twopi );
    if ( meananomaly < 0.0 )
        meananomaly = twopi + meananomaly;
    end

    loneclsun = meanlong + 1.914666471 * sin(meananomaly)  ...
        + 0.019994643 * sin(2.0 * meananomaly);

    loneclmoon = 218.32 + 481267.8813 * ttdb + 6.29 * sin( (134.9 + 477198.85 * ttdb)  ...
        * deg2rad ) - 1.27 * sin( (259.2 -413335.38 * ttdb) * deg2rad ) ...
        + 0.66 * sin( (235.7 + 890534.23 * ttdb) * deg2rad )  ...
        + 0.21 * sin( (269.9 + 954397.70 * ttdb) * deg2rad )  ...
        - 0.19 * sin( (357.5 + 35999.05 * ttdb) * deg2rad )  ...
        - 0.11 * sin( (186.6 + 966404.05 * ttdb) * deg2rad );
    loneclmoon = rem( loneclmoon, 360.0  );

    moonphaseang = loneclmoon - loneclsun;

    if ( moonphaseang < 0.0  )
        moonphaseang = 360.0  + moonphaseang;
    end

