    %     -----------------------------------------------------------------
    %
    %                              Ex3_10.m
    %
    %  this file demonstrates example 3-10.
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
    hr = 13;
    min = 22;
    sec = 45.98;
    fprintf(1,'hr %4i ',hr);
    fprintf(1,'min %4i ',min);
    fprintf(1,'sec %8.6f \n',sec);

    [utsec] = hms2sec( hr,min,sec );

    fprintf(1,'hms %11.7f \n',utsec);
    
    temp   = utsec   / 3600.0;
    hr  = fix( temp  );
    min = fix( (temp - hr)*60.0 );
    sec = (temp - hr - min/60.0 ) * 3600.0;
        
    fprintf(1,' hr min sec %4i  %4i  %8.6f \n',hr, min, sec);
    
