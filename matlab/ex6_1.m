    %     -----------------------------------------------------------------
    %
    %                              ex6_1.m
    %
    %  this file demonstrates example 6-1.
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
    fprintf(1,'-------------------- problem ex 6-1 \n');
    rinit  = (re + 191.3411)/re;
    rfinal = (re + 35781.34857)/re;
    einit  = 0.0;
    efinal = 0.0;
    nuinit = 0.0/rad;
    nufinal= 180.0/rad;

    fprintf(1,' rinit  %11.6f %11.6f km \n',rinit, rinit*re);
    fprintf(1,' rfinal %11.6f %11.6f km \n',rfinal, rfinal*re);
    fprintf(1,' einit   %11.6f \n',einit);
    fprintf(1,' efinal  %11.6f \n',efinal);
    fprintf(1,' nuinit  %11.6f deg \n',nuinit * rad);
    fprintf(1,' nufinal %11.6f deg \n',nufinal * rad);

    [deltava,deltavb,dttu ] = hohmann (rinit,rfinal,einit,efinal,nuinit,nufinal);

    constastro;
    fprintf(1,'hohmann answers \n');
    fprintf(1,' deltava  %11.6f  %11.6f km/s \n',deltava, deltava*velkmps );
    fprintf(1,' deltavb  %11.6f  %11.6f km/s \n',deltavb, deltavb*velkmps );
    fprintf(1,' dttu  %11.6f tu %11.6f hr %11.6f min \n',dttu,dttu*tumin/60.0, dttu*tumin);






