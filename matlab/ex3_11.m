    %     -----------------------------------------------------------------
    %
    %                              Ex3_11.m
    %
    %  this file demonstrates example 3-11.
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

    [jd, jdfrac] = jday(1992, 5, 8, 0, 0, 0.0);
    fprintf(1,'input jd %14.7f %11.8f  \n\n', jd, jdfrac );

     % check jdfrac for multiple days
     if abs(jdfrac) >= 1.0 
         jd = jd + floor(jdfrac);
         jdfrac = jdfrac - floor(jdfrac);
     end

     % check for fraction of a day included in the jd
	 dt = jd - floor(jd) - 0.5;
     if (abs(dt) > 0.00000001)
         jd = jd - dt;  
         jdfrac = jdfrac + dt;
     end

     % ----------------- find year and days of the year ---------------
     temp   = jd - 2415019.5; 
     tu     = temp / 365.25;
     year   = 1900 + floor( tu );
     leapyrs= floor( ( year-1901 )*0.25 );
     days   = floor(temp - ((year-1900)*365.0 + leapyrs ));

     % ------------ check for case of beginning of a year -------------
     if days + jdfrac < 1.0
         year   = year - 1;
         leapyrs= floor( ( year-1901 )*0.25 );
         days   = floor(temp - ((year-1900)*365.0 + leapyrs ));
     end

    fprintf(1,'year %6i  \n',year);
    fprintf(1,'mon  %3i  \n',mon);
    fprintf(1,'days  %3i  \n',days);

