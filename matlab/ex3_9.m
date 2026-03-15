    %     -----------------------------------------------------------------
    %
    %                              Ex3_9.m
    %
    %  this file demonstrates example 3-9.
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

    % -------- hms test
    hr = 15;
    min = 15;
    sec = 53.63;
    fprintf(1,'hr %4i ',hr);
    fprintf(1,'min %4i ',min);
    fprintf(1,'sec %8.6f \n',sec);

    [hms] = hms2rad( hr,min,sec );

    fprintf(1,'hms %11.7f %11.7f \n',hms, hms * 180.0/pi);

    [hr,min,sec] = rad2hms( hms );

    fprintf(1,' hr min sec %4i  %4i  %8.6f \n',hr, min, sec);

