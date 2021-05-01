    %     -----------------------------------------------------------------
    %
    %                              ex6_2.m
    %
    %  this file demonstrates example 6-2.
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
    fprintf(1,'-------------------- problem ex 6-2 \n');
    rinit  = (re + 191.3411)/re;
    rb     = (re + 503873.0)/re;
    rfinal = (re + 376310.0)/re;
    einit  = 0.0;
    efinal = 0.0;
    nuinit = 0.0/rad;
    nufinal= 180.0/rad;

    constastro;
    fprintf(1,'initial position \n');
    fprintf(1,' rinit  %11.7f %11.7f km \n',rinit, rinit*re);
    fprintf(1,' rfinal %11.7f %11.7f km \n',rfinal, rfinal*re);
    fprintf(1,' einit   %11.7f \n',einit);
    fprintf(1,' efinal  %11.7f \n',efinal);
    fprintf(1,' nuinit  %11.7f deg \n',nuinit * rad);
    fprintf(1,' nufinal %11.7f deg \n',nufinal * rad);

    [deltava,deltavb,deltavc,dttu ] = biellip(rinit,rb,rfinal,einit,efinal,nuinit,nufinal);


    constastro;
    fprintf(1,'bi-elliptic answers \n');
    fprintf(1,' deltava  %11.7f  %11.7f km/s \n',deltava, deltava*velkmps );
    fprintf(1,' deltavb  %11.7f  %11.7f km/s \n',deltavb, deltavb*velkmps );
    fprintf(1,' deltavc  %11.7f  %11.7f km/s \n',deltavc, deltavc*velkmps );
    fprintf(1,' dttu  %11.6f tu %11.6f hr %11.6f min \n',dttu,dttu*tumin/60.0, dttu*tumin);
