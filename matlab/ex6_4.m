    %     -----------------------------------------------------------------
    %
    %                              ex6_4.m
    %
    %  this file demonstrates example 6-4.
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
    mu = 1.0;  % canonical

    fprintf(1,'-------------------- problem ex 6-4 \n');
    deltai = 15.0 / rad;
    vinit = 5.892311 / velkmps;  % er/tu
    incl = 28.5/rad;
    fpa = 0.0/rad;

    [deltavionly] = ionlychg(deltai,vinit,fpa);

    fprintf(1,'inclination only changes \n');
    fprintf(1,' vinit   %11.7f  %11.7f \n\n',vinit, vinit*velkmps );
    fprintf(1,' deltavionly  %11.7f  %11.7f \n\n',deltavionly, deltavionly*velkmps );
    fprintf(1,' comp  %11.7f  %11.7f \n\n',-deltavionly*velkmps*cos(deltai), deltavionly*velkmps*sin(deltai) );


    fprintf(1,'-----part 2 \n');
    deltai = 15.0 / rad;
    ecc = 0.3;
    p = 17858.7836 / re;
    nu = 330.0 / rad;
    a = p / (1.0 - ecc * ecc);
    r = p / (1.0 + ecc * cos(nu) );
    vinit  = sqrt( (2.0* mu)/r - (mu/a) );
    fpa = atan((ecc * sin(nu)) / (1.0 + ecc * cos(nu)));

    [deltavionly] = ionlychg(deltai,vinit,fpa);

    fprintf(1,' a  %11.7f  %11.7f km r %11.7f  %11.7f km fpa %11.7f \n\n',a, a*re, r, r*re, fpa*rad );
    fprintf(1,'inclination only changes \n');
    fprintf(1,' vinit   %11.7f  %11.7f \n\n',vinit, vinit*velkmps );
    fprintf(1,' deltavionly   %11.7f  %11.7f \n\n',deltavionly, deltavionly*velkmps );
    fprintf(1,' comp  %11.7f  %11.7f \n\n',-deltavionly*velkmps*cos(deltai), deltavionly*velkmps*sin(deltai) );


    fprintf(1,'-----part 3 \n');
    deltai = 15.0 / rad;
    ecc = 0.3;
    p = 17858.7836 / re;
    nu = 150.0 / rad;
    a = p / (1.0 - ecc * ecc);
    r = p / (1.0 + ecc * cos(nu) );
    vinit  = sqrt( (2.0* mu)/r - (mu/a) );
    fpa = atan((ecc * sin(nu)) / (1.0 + ecc * cos(nu)));

    [deltavionly] = ionlychg(deltai,vinit,fpa);

    fprintf(1,' a  %11.7f  %11.7f km r %11.7f  %11.7f km fpa %11.7f \n\n',a, a*re, r, r*re, fpa*rad );
    fprintf(1,'inclination only changes \n');
    fprintf(1,' deltavionly   %11.7f  %11.7f \n\n',deltavionly, deltavionly*velkmps );
    fprintf(1,' vinit   %11.7f  %11.7f \n\n',vinit, vinit*velkmps );
    fprintf(1,' comp  %11.7f  %11.7f \n\n',-deltavionly*velkmps*cos(deltai), deltavionly*velkmps*sin(deltai) );


    deltai = (90.0 - 28.5) / rad;
    vinit = sqrt(398600.418/6600) / velkmps;  % er/tu
    incl = 28.5/rad;
    fpa = 0.0/rad;

    [deltavionly] = ionlychg(deltai,vinit,fpa);

    fprintf(1,'inclination only changes \n');
    fprintf(1,' vinit   %11.7f  %11.7f \n\n',vinit, vinit*velkmps );
    fprintf(1,' deltavionly  %11.7f  %11.7f \n\n',deltavionly, deltavionly*velkmps );
    fprintf(1,' comp  %11.7f  %11.7f \n\n',-deltavionly*velkmps*cos(deltai), deltavionly*velkmps*sin(deltai) );


