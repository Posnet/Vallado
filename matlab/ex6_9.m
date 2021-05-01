    %     -----------------------------------------------------------------
    %
    %                              ex6_9.m
    %
    %  this file demonstrates example 6-9.
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

    fprintf(1,'-------------------- problem ex 6-9 \n');
    rcs1 = 12756.274 / re;
    rcs3 = 42159.48 / re;
    phasei = -20.0 / rad;
    einit = 0.0;
    efinal = 0.0;
    nuinit = 0.0 / rad;
    nufinal = 0.0 / rad;
    ktgt = 1;
    kint = 1;

    fprintf(1,'\n rendezvous ktgt %i kint %i \n', ktgt, kint);
    [ phasef,waittime,deltav] = rendz(rcs1,rcs3,phasei,einit,efinal,nuinit,nufinal,ktgt,kint);

    fprintf(1,' phasef %11.7f  %11.7f  \n',phasef * rad, phasef);
    fprintf(1,' waittime %11.7f  %11.7f min  \n',waittime, waittime*tumin);
    fprintf(1,' deltav %11.7f  %11.7f km/s \n',deltav, deltav*velkmps);

