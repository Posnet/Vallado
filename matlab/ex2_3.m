    %     -----------------------------------------------------------------
    %
    %                              Ex2_3.m
    %
    %  this file demonstrates example 2-3.
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
    constastro;

    % --------  newtonm      - find hyperbolic and true anomaly given ecc and mean anomaly
    ecc = 2.4;
    m = 235.4/rad;

    fprintf(1,'               m                e           nu           h   \n');

    [h ,nu]= newtonm(ecc, m);
    
    fprintf(1,'newm  %14.8f %14.8f %14.8f %14.8f  rad \n',m, ecc, nu, h );
    fprintf(1,'newm  %14.8f %14.8f %14.8f %14.8f  deg \n',m*rad, ecc, nu*rad, h*rad );

