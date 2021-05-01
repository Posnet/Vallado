%     -----------------------------------------------------------------
%
%                              Ex7_5.m
%
%  this file implements example 7-5.
%
%                          companion code for
%             fundamentals of astrodynamics and applications
%                                 2020
%                            by david vallado
%
%     (w) 719-573-2600, email dvallado@agi.com
%
%     *****************************************************************
%
%  current :
%            26 may 20  david vallado
%                         separate from temp codes.
%  changes :
%            13 feb 07  david vallado
%                         original baseline
%     *****************************************************************

    fid = 1;    
    directory = 'd:\codes\library\matlab\';
    outfile = fopen(strcat(directory,'tlambfig.out'), 'wt');
    
    constastro;

    % ---------------------------- book tests -----------------------------
    r1 = [ 2.500000,    0.000000 ,   0.000000]*re;
    r2 = [ 1.9151111,   1.6069690,   0.000000]*re;
    
    % original orbit, assume circular
    v1 = [0 0 0];
    fprintf(1,'\n-------- lambert test book pg 497 short way \n' );
    v1(2) = sqrt(mu/r1(1));
    ang = atan(r2(2)/r2(1));
    v2 = [-sqrt(mu/r2(2))*cos(ang);sqrt(mu/r2(1))*sin(ang);0.0];
    fprintf(1,'\n v1 %16.8f%16.8f%16.8f\n',v1 );
    fprintf(1,'\n v2 %16.8f%16.8f%16.8f\n',v2 );
    dtsec = 76.0*60.0;
    dtsec = 21300.0000; 
  
    pause;
    % now show all the cases
    
        magr1 = mag(r1);
    magr2 = mag(r2);

    % this value stays constant in all calcs, vara changes with df
    cosdeltanu = dot(r1,r2) / (magr1*magr2);  
    
    dm = 's';
    df = 'd';
    nrev = 0;
    [ tbidu, tbiru] = lambgettbiu(r1, r2, 5);
    fprintf(1,' r1 %16.8f%16.8f%16.8f\n',r1 );
    fprintf(1,' r2 %16.8f%16.8f%16.8f\n',r2 );
    fprintf(1,'From universal variables \n%11.7f %11.7f s \n',tbidu(1,1),tbidu(1,2));
    fprintf(1,'%11.7f %11.7f s \n',tbidu(2,1),tbidu(2,2));
    fprintf(1,'%11.7f %11.7f s \n',tbidu(3,1),tbidu(3,2));
    fprintf(1,'%11.7f %11.7f s \n',tbidu(4,1),tbidu(4,2));
    fprintf(1,'%11.7f %11.7f s \n\n',tbidu(5,1),tbidu(5,2));
    fprintf(1,'%11.7f %11.7f s \n',tbiru(1,1),tbiru(1,2));
    fprintf(1,'%11.7f %11.7f s \n',tbiru(2,1),tbiru(2,2));
    fprintf(1,'%11.7f %11.7f s \n',tbiru(3,1),tbiru(3,2));
    fprintf(1,'%11.7f %11.7f s \n',tbiru(4,1),tbiru(4,2));
    fprintf(1,'%11.7f %11.7f s \n',tbiru(5,1),tbiru(5,2));

    [minenergyv, aminenergy, tminenergy, tminabs] = lambertmin ( r1, r2, 'd', 0 );
    fprintf(1,' minenergyv %16.8f %16.8f %16.8f a %11.7f  dt %11.7f  %11.7f \n', minenergyv, aminenergy, tminenergy, tminabs );
     
    [minenergyv, aminenergy, tminenergy, tminabs] = lambertmin ( r1, r2, 'r', 0 );
    fprintf(1,' minenergyv %16.8f %16.8f %16.8f a %11.7f  dt %11.7f  %11.7f \n', minenergyv, aminenergy, tminenergy, tminabs );

    
    dtwait = 0.0;
    fprintf(1,'\n-------- lambertu test \n' );
    [v1t, v2t, errorl] = lambertu ( r1, v1, r2, dm, df,nrev, dtwait, dtsec, tbidu, fid );
    fprintf(1,' v1t %16.8f%16.8f%16.8f\n',v1t );
    fprintf(1,' v2t %16.8f%16.8f%16.8f\n',v2t );
    
    % run the 6 cases
   % dtsec = 21300.0;
       fprintf(1,' TEST ------------------ s  d  0 rev \n');
    [v1t, v2t, errorl] = lambertu ( r1, v1, r2, 's', 'd', 0, dtwait, dtsec, tbidu, fid );
    fprintf(1,' v1t %16.8f%16.8f%16.8f\n',v1t );
    fprintf(1,' v2t %16.8f%16.8f%16.8f\n',v2t );

       fprintf(1,' TEST ------------------ l  r  0 rev \n');
    [v1t, v2t, errorl] = lambertu ( r1, v1, r2, 'l', 'r', 0, dtwait, dtsec, tbiru, fid );
    fprintf(1,' v1t %16.8f%16.8f%16.8f\n',v1t );
    fprintf(1,' v2t %16.8f%16.8f%16.8f\n',v2t );

       fprintf(1,' TEST ------------------ s  d  1 rev \n');
    [v1t, v2t, errorl] = lambertu ( r1, v1, r2, 's', 'd', 1, dtwait, dtsec, tbidu, fid );
    fprintf(1,'uv1t %16.8f%16.8f%16.8f\n',v1t );
    fprintf(1,' v2t %16.8f%16.8f%16.8f\n',v2t );
    
       fprintf(1,' TEST ------------------ s  r  1 rev \n');
    [v1t, v2t, errorl] = lambertu ( r1, v1, r2, 's', 'r', 1, dtwait, dtsec, tbiru, fid );
    fprintf(1,'uv1t %16.8f%16.8f%16.8f\n',v1t );
    fprintf(1,' v2t %16.8f%16.8f%16.8f\n',v2t );
    
       fprintf(1,' TEST ------------------ l  d  1 rev \n');
    [v1t, v2t, errorl] = lambertu ( r1, v1, r2, 'l', 'd', 1, dtwait, dtsec, tbidu, fid );
    fprintf(1,'uv1t %16.8f%16.8f%16.8f\n',v1t );
    fprintf(1,' v2t %16.8f%16.8f%16.8f\n',v2t );
    
       fprintf(1,' TEST ------------------ l  r  1 rev \n');
    [v1t, v2t, errorl] = lambertu ( r1, v1, r2, 'l', 'r', 1, dtwait, dtsec, tbiru, fid );
    fprintf(1,'uv1t %16.8f%16.8f%16.8f\n',v1t );
    fprintf(1,' v2t %16.8f%16.8f%16.8f\n',v2t );

    fprintf(1,'\n-------- lambertb test \n' );
    [v1t, v2t, errorl] = lambertb ( r1, v1, r2, dm, df, nrev, dtsec );
    fprintf(1,' v1dv %16.8f%16.8f%16.8f\n',v1t );
    fprintf(1,' v2dv %16.8f%16.8f%16.8f\n',v2t );


    



    