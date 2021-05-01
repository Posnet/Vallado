    %     -----------------------------------------------------------------
    %
    %                              ex6_12.m
    %
    %  this file demonstrates example 6-12.
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

    constastro;

    rad = 180.0 / pi;

    fprintf(1,'-------------------- problem ex 6-12 \n');
    rinit  = (re + 318.9);  % in km
    rfinal = (re + 35781.343);
    einit  = 0.0;
    efinal = 0.0;
    iinit = 0.0/rad;
    ifinal= 0.0/rad;

    fprintf(1,' rinit  %11.7f m %11.7f er \n',rinit, rinit/re);
    fprintf(1,' rfinal %11.7f m %11.7f er \n',rfinal, rfinal/re);
    fprintf(1,' einit   %11.7f \n',einit);
    fprintf(1,' efinal  %11.7f \n',efinal);
    fprintf(1,' iinit   %11.7f deg \n',iinit * rad);
    fprintf(1,' ifinal  %11.7f deg \n',ifinal * rad);

    [deltav, tof] = lowthrust(rinit, rfinal, iinit, ifinal, 5000.0, -2.48e-7, 4.0e-6, 0.001);

    constastro;
    fprintf(1,'low thrust answers \n');
    fprintf(1,' deltav  %11.7f km/s  \n',deltav );
    fprintf(1,' dttu  %11.7f day %11.7f min \n',tof, tof*1440);

