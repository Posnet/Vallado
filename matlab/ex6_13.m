    %     -----------------------------------------------------------------
    %
    %                              ex6_13.m
    %
    %  this file demonstrates example 6-13.
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

    fprintf(1,'-------------------- problem ex 6-13 \n');
    rinit  = (re + 200.0);  % in km
    rfinal = (re + 35781.343);
    einit  = 0.0;
    efinal = 0.0;
    iinit = 28.5/rad;
    ifinal= 0.0/rad;

    fprintf(1,' rinit  %11.7f m %11.7f er \n',rinit, rinit/re);
    fprintf(1,' rfinal %11.7f m %11.7f er \n',rfinal, rfinal/re);
    fprintf(1,' einit   %11.7f \n',einit);
    fprintf(1,' efinal  %11.7f \n',efinal);
    fprintf(1,' iinit   %11.7f deg \n',iinit * rad);
    fprintf(1,' ifinal  %11.7f deg \n',ifinal * rad);

    % book example
    constastro;
    du = 6578.136
    du1 = du/6378.136
    tu=sqrt(du1^3/1.0)*tusec
    ratio = rfinal/rinit
    vacc = 0.75*du/tu
    tof=-1/(-2e-7) * (1-exp((-2e-7*5.8382)/1e-5))

    pause;

    %[deltav, tof] = lowthrust(rinit, rfinal, iinit, ifinal, 3800.0, 0.456, 0.234876);
    [deltav, tof] = lowthrust(rinit, rfinal, iinit, ifinal, 5000.0, -2.0e-7, 1.0e-5, -0.54);
    %low_thrust_transfer_prop(rinit, rfinal, iinit, ifinal, 3800.0, 0.456, 0.234876);

    constastro;
    fprintf(1,'low thrust answers \n');
    fprintf(1,' deltav  %11.7f  km/s \n',deltav );
    fprintf(1,' dttu  %11.7f day %11.7f min \n',tof, tof*1440);

