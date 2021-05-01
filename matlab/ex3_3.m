    %     -----------------------------------------------------------------
    %
    %                              Ex3_3.m
    %
    %  this file demonstrates example 3-3.
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

    constmath;

    % --------  ecef2ll       - position to lat lon alt almanac (fastest)
    r1=[6524.834 6862.875 6448.296];

    [latgc,latgd,lon,hellp] = ecef2ll ( r1 );

    fprintf(1,'ecef2ll  gc %14.7f gd %14.7f %14.7f%14.7f\n',latgc*rad,latgd*rad,lon*rad,hellp );

    % --------  ecef2llb      - position to lat lon alt borkowski
    [latgc,latgd,lon,hellp] = ecef2llb ( r1 );

    fprintf(1,'ecef2llb gc %14.7f gd %14.7f %14.7f%14.7f\n',latgc*rad,latgd*rad,lon*rad,hellp );

    % try another case for testing the borkowski method 
    % Latitude
    deg =  -7;
    min =  -54;
    sec = -23.886;
    latgd = dms2rad(deg,min,sec);

    % Longitude
    deg = 345;
    min =  35;
    sec =  51.000;
    lon = dms2rad(deg,min,sec);

    % Altitude
    hellp = 0.056;
    rs = site(latgd,lon, hellp);

    [latgc,latgd,lon,hellp] = ecef2ll(rs);

    fprintf(1,'ecef2ll  gc %14.7f gd %14.7f %14.7f%14.7f\n',latgc*rad,latgd*rad,lon*rad,hellp );

    [latgcb,latgdb,lonb,hellpb] = ecef2llb(rs);

    fprintf(1,'ecef2llb gc %14.7f gd %14.7f %14.7f%14.7f\n',latgcb*rad,latgdb*rad,lonb*rad,hellpb );

    % check for any imaginary numbers
    latgcb
    latgdb
    lonb
    hellpb
