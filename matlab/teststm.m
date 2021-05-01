%
%   teststm
%
% test the various implementations of the stm moving a state through time
%
%
%
        directory = 'd:\codes\library\matlab\';
        outfile = fopen(strcat(directory,'teststm.out'), 'wt');
 
        rad = 180.0/pi;
        mu = 398600.4418;
        
        rorig = [-605.79221660 -5870.22951108 3493.05319896];
        vorig = [-1.56825429 -3.70234891 -6.47948395 ];

   %     ro = [-4550.4256  2220.1946  5090.3547];
   %     vo = [-4.975006  -5.237109  -2.11811];

%  test pg 103
%       ro = [1131.34  -2282.343   6672.423];
%       vo = [-5.64305   4.30333  2.42879];

        year = 1999;
        mon  =   1;
        day  =  15;
        hr   =   0;
        min  =   0;
        sec  =   0.0;

        % now do two-body prop to get correct answers
       fprintf(outfile,'  two body prop   \n');
       fprintf(1,'  two body prop   \n');
       ro = rorig;
       vo = vorig;
        for i = 1:10
            dtseco = i*100;            
            [r,v] = kepler  ( ro,vo, dtseco );
            
            magv = mag(v);
            c1 = magv*magv - mu /mag(r);  % mu
            rdotv= dot( r,v );
            for ii= 1 : 3
                ebar(ii)= (c1*r(ii) - rdotv*v(ii))/mu;
            end
            nu1 =  angl( ebar,r);
            if ( rdotv < 0.0  )
                nu1 = 2*pi - nu1;
            end
            edotr= dot( ebar,r );
            fprintf(outfile,'%11.7f  %11.7f  %11.7f', nu1*rad, rdotv, edotr);        
            
            rans(i,:) = r;  % put each vector in a row
            fprintf(outfile,'%6f r %11.7f  %11.7f  %11.7f  %11.7f  %11.7f  %11.7f   \n',dtseco,r, v );
        end;
       
        stm=zeros(6,6);
        fprintf(1,'---- case 1: pg 813 last eq epoch, i + fdt  + f2dt2/2 + f3dt3/6 + f4dt4/24 \n');
        fprintf(outfile,'---- case 1: pg 813 last eq epoch, i + fdt  + f2dt2/2 + f3dt3/6 + f4dt4/24 \n');
       ro = rorig;
       vo = vorig;
        magro = mag(ro);
        for i = 1:10
            dt = i*100; % secs
            x = [ro vo]';
            g = zeros(6,6);
%             g(1,1) = -mu/(2.0*magro^3);
%             g(2,2) = -mu/(2.0*magro^3);
%             g(3,3) = -mu/(2.0*magro^3);
            g(1,4) = 1.0;
            g(2,5) = 1.0;
            g(3,6) = 1.0;
            g(4,1) = -mu/magro^3;
            g(5,2) = -mu/magro^3;
            g(6,3) = -mu/magro^3;
            stm = eye(6) + g*dt + 0.5*g*g*dt^2 + 1.0/6.0 * g*g*g*dt^3 + 1.0/24.0 * g*g*g*g*dt^4;
            if i == 1
                stm
            end
            x1 = stm * x;
            r(1) = x1(1);
            r(2) = x1(2);
            r(3) = x1(3);  
            dr = rans(i,:) - r;
            fprintf(outfile,'%6i x %14.7f  %14.7f  %14.7f  %14.7f  %14.7f  %14.7f dr %9.4f km\n',dt,x1,mag(dr) );
        end;

                stm=zeros(6,6);
        fprintf(1,'---- case 1a: pg 813 last eq step by step, i + fdt  + f2dt2/2 + f3dt3/6 + f4dt4/24 \n');
        fprintf(outfile,'---- case 1a: pg 813 last eq step by step, i + fdt  + f2dt2/2 + f3dt3/6 + f4dt4/24 \n');
       ro = rorig;
       vo = vorig;
        magro = mag(ro);
        for i = 1:10
            dt = 100; % secs
            x = [ro vo]';
            g = zeros(6,6);
%             g(1,1) = -mu/(2.0*magro^3);
%             g(2,2) = -mu/(2.0*magro^3);
%             g(3,3) = -mu/(2.0*magro^3);
            g(1,4) = 1.0;
            g(2,5) = 1.0;
            g(3,6) = 1.0;
            g(4,1) = -mu/magro^3;
            g(5,2) = -mu/magro^3;
            g(6,3) = -mu/magro^3;
            stm = eye(6) + g*dt + 0.5*g*g*dt^2 + 1.0/6.0 * g*g*g*dt^3 + 1.0/24.0 * g*g*g*g*dt^4;
            if i == 1
                stm
            end
            x1 = stm * x;
            r(1) = x1(1);
            r(2) = x1(2);
            r(3) = x1(3);  
            v(1) = x1(4);
            v(2) = x1(5);
            v(3) = x1(6);  
            dr = rans(i,:) - r;
            ro = r;
            vo = v;
            fprintf(outfile,'%6i x %14.7f  %14.7f  %14.7f  %14.7f  %14.7f  %14.7f dr %9.4f km\n',dt,x1,mag(dr) );
        end;

        
        stm=zeros(6,6);
        fprintf(1,'---- case 2: pg 813 eq 10-46 epoch state x = stm*xo \n');
        fprintf(outfile,'---- case 2: pg 813 eq 10-46 epoch state x = stm*xo \n');
       ro = rorig;
       vo = vorig;
        magro = mag(ro);
        for i = 1:10
  %          stm = stm2(ro,dt);
            dt = i*100; % secs
            x = [ro vo]';
            stm = zeros(6,6);
            stm(1,4) = dt;
            stm(2,5) = dt;
            stm(3,6) = dt;
            stm(1,1) = 1.0 -mu*dt^2/(2.0*magro^3);
            stm(2,2) = 1.0 -mu*dt^2/(2.0*magro^3);
            stm(3,3) = 1.0 -mu*dt^2/(2.0*magro^3);
            stm(4,4) = 1.0;
            stm(5,5) = 1.0;
            stm(6,6) = 1.0;
            stm(4,1) = -mu*dt/magro^3;
            stm(5,2) = -mu*dt/magro^3;
            stm(6,3) = -mu*dt/magro^3;
            %stm = (eye(6) + g*dt + 0.5*g*g*dt^2); % + 1.0/6.0 * g*g*g*dt^3);
            if i == 1
                stm
            end
            x1 = stm*x;

            r(1) = x1(1);
            r(2) = x1(2);
            r(3) = x1(3);  
            dr = rans(i,:) - r;
            fprintf(outfile,'%6i x %14.7f  %14.7f  %14.7f  %14.7f  %14.7f  %14.7f dr %9.4f km\n',dt,x1,mag(dr) );
        end;

        stm=zeros(6,6);
        fprintf(1,'---- case 2a: pg 813 eq 10-46 step by step state x = stm*xo \n');
        fprintf(outfile,'---- case 2a: pg 813 eq 10-46 step by step state x = stm*xo \n');
       ro = rorig;
       vo = vorig;
        magro = mag(ro);
        for i = 1:10
  %          stm = stm2(ro,dt);
            dt = 100; % secs
            x = [ro vo]';
            stm = zeros(6,6);
            stm(1,4) = dt;
            stm(2,5) = dt;
            stm(3,6) = dt;
            stm(1,1) = 1.0 -mu*dt^2/(2.0*magro^3);
            stm(2,2) = 1.0 -mu*dt^2/(2.0*magro^3);
            stm(3,3) = 1.0 -mu*dt^2/(2.0*magro^3);
            stm(4,4) = 1.0;
            stm(5,5) = 1.0;
            stm(6,6) = 1.0;
            stm(4,1) = -mu*dt/magro^3;
            stm(5,2) = -mu*dt/magro^3;
            stm(6,3) = -mu*dt/magro^3;
            %stm = (eye(6) + g*dt + 0.5*g*g*dt^2); % + 1.0/6.0 * g*g*g*dt^3);
            if i == 1
                stm
            end
            x1 = stm * x;
            r(1) = x1(1);
            r(2) = x1(2);
            r(3) = x1(3);  
            v(1) = x1(4);
            v(2) = x1(5);
            v(3) = x1(6);  
            dr = rans(i,:) - r;
            ro = r;
            vo = v;
            fprintf(outfile,'%6i x %14.7f  %14.7f  %14.7f  %14.7f  %14.7f  %14.7f dr %9.4f km\n',dt,x1,mag(dr) );
        end;
        
        
        
        stm=zeros(6,6);
        fprintf(1,'---- case 3: simple euler x = xo + xdot*dt + xdot*dt2/2\n');
        fprintf(outfile,'---- case 3: simple euler x = xo + xdot*dt + xdot*dt2/2\n');
        fprintf(outfile,'at least %3.2f sec step size for accuracy \n',dt);
       ro = rorig;
       vo = vorig;
        x = [ro vo]';
        for i = 0:10
            [stmv,stma] = stm3(ro,vo);
            if i == 1
                stmv
                stma
            end
            dt = i*100; % secs
            x = [ro vo]';
            x1 = stmv*dt + 0.5*stma*dt*dt;
            x1 = x1' + x;

%            if (mod(i,600) == 0) | (i == 0)
                fprintf(outfile,'%4i x %11.7f  %11.7f  %11.7f  %11.7f  %11.7f  %11.7f   \n',dt,x1 );
%            end;
        end;

        stm=zeros(6,6);
        fprintf(1,'---- case 4: epoch pg 110 escobal f and g series \n');
        fprintf(outfile,'---- case 4: epoch pg 110 escobal f and g series \n');
        fprintf(outfile,'at least %3.2f sec step size for accuracy \n',dt);
       ro = rorig;
       vo = vorig;
       magro = mag(ro);
        x = [ro vo]';
        fprintf(outfile,'  0 x %11.7f  %11.7f  %11.7f  %11.7f  %11.7f  %11.7f   \n',x );
        for i = 1:10
            dt = i*100; % secs
            x = [ro vo]';
            
            u = mu/magro^3;
            p = dot(ro, vo) / magro^2;
            q = (mag(vo)^2 - magro^2*u) / magro^2;
            f = 1.0 - 0.5*u*dt^2 + 0.5*u*p*dt^3 + 1.0/24.0*(3*u*q - 15*u*p^2 + u^2)*dt^4 + ...
                1.0/8.0*(7*u*p^3 - 3*u*p*q - u^2*p)*dt^5 +  ...
                1.0/720.0*(630*u*p^2*q - 24*u^2*q - u^3 - 45*u*q^2 - 945*u*p^4 + 210*u^2*p^2)*dt^6 + ...
                1.0/5040*(882*u^2*p*q - 3150*u^2*p^3 - 9450*u*p^3*q + 1575*u*p*q^2 + 63*u^3*p + 10395*u*p^5)*dt^7 + ...
                1.0/40320*(1107*u^2*q^2 - 24570*u^2*p^2*q - 2205*u^3*p^2 + 51975*u^2*p^4 - 42525*u*p^2*q^2 + 155925*u*p^4*q + 1575*u*q^3 + 117*u^3*q - 135135*u*p^6 + u^4)*dt^8;

            g = dt - 1/6*u*dt^3 + 0.25*u*p*dt^4 + 1/120*(9*u*q - 45*u*p^2 + u^2)*dt^5 + ...
                1/360*(210*u*p^3 - 90*u*p*q - 15*u^2*p)*dt^6 + ...
                1/5040*(3150*u*p^2*q - 54*u^2*q - 225*u*q^2 - 4725*u*p^4 + 630*u^2*p^2 - u^3)*dt^7 + ...
                1/40320*(3024*u^2*p*q - 12600*u^2*p^3 - 56700*u*p^3*q + 9450*u*p*q^2 + 62370*u*p^5 + 126*u^3*p)*dt^8;
            
            fdot = -u*dt + 1.5*u*p*dt^2 + 1.0/6.0*(3*u*q - 15*u*p^2 + u^2)*dt^3 + ...
                5.0/8.0*(7*u*p^3 - 3*u*p*q - u^2*p)*dt^4 +  ...
                6.0/720.0*(630*u*p^2*q - 24*u^2*q - u^3 - 45*u*q^2 - 945*u*p^4 + 210*u^2*p^2)*dt^5 + ...
                7.0/5040*(882*u^2*p*q - 3150*u^2*p^3 - 9450*u*p^3*q + 1575*u*p*q^2 + 63*u^3*p + 10395*u*p^5)*dt^6 + ...
                8.0/40320*(1107*u^2*q^2 - 24570*u^2*p^2*q - 2205*u^3*p^2 + 51975*u^2*p^4 - 42525*u*p^2*q^2 + 155925*u*p^4*q + 1575*u*q^3 + 117*u^3*q - 135135*u*p^6 + u^4)*dt^7;

            gdot = 1 - 0.5*u*dt^2 + u*p*dt^3 + 5/120*(9*u*q - 45*u*p^2 + u^2)*dt^4 + ...
                1/60*(210*u*p^3 - 90*u*p*q - 15*u^2*p)*dt^5 + ...
                7/5040*(3150*u*p^2*q - 54*u^2*q - 225*u*q^2 - 4725*u*p^4 + 630*u^2*p^2 - u^3)*dt^6 + ...
                8/40320*(3024*u^2*p*q - 12600*u^2*p^3 - 56700*u*p^3*q + 9450*u*p*q^2 + 62370*u*p^5 + 126*u^3*p)*dt^7;

            stm(1,1) = f;
            stm(2,2) = f;
            stm(3,3) = f;
            stm(1,4) = g;
            stm(2,5) = g;
            stm(3,6) = g;
            stm(4,1) = fdot;
            stm(5,2) = fdot;
            stm(6,3) = fdot;
            stm(4,4) = gdot;
            stm(5,5) = gdot;
            stm(6,6) = gdot;
            if i == 1
                stm
            end
            x1 = stm*x;

            r(1) = x1(1);
            r(2) = x1(2);
            r(3) = x1(3);  
            dr = rans(i,:) - r;
            fprintf(outfile,'%6i x %14.7f  %14.7f  %14.7f  %14.7f  %14.7f  %14.7f dr %9.4f km\n',dt,x1,mag(dr) );
        end;

        stm=zeros(6,6);
        fprintf(1,'---- case 4a: step by step pg 110 escobal f and g series \n');
        fprintf(outfile,'---- case 4a: step by step pg 110 escobal f and g series \n');
        fprintf(outfile,'at least %3.2f sec step size for accuracy \n',dt);
       ro = rorig;
       vo = vorig;
       magro = mag(ro);
        x = [ro vo]';
        fprintf(outfile,'  0 x %11.7f  %11.7f  %11.7f  %11.7f  %11.7f  %11.7f   \n',x );
        for i = 1:10
            dt = 100; % secs
            x = [ro vo]';
            
            u = mu/magro^3;
            p = dot(ro, vo) / magro^2;
            q = (mag(vo)^2 - magro^2*u) / magro^2;
            f = 1.0 - 0.5*u*dt^2 + 0.5*u*p*dt^3 + 1.0/24.0*(3*u*q - 15*u*p^2 + u^2)*dt^4 + ...
                1.0/8.0*(7*u*p^3 - 3*u*p*q - u^2*p)*dt^5 +  ...
                1.0/720.0*(630*u*p^2*q - 24*u^2*q - u^3 - 45*u*q^2 - 945*u*p^4 + 210*u^2*p^2)*dt^6 + ...
                1.0/5040*(882*u^2*p*q - 3150*u^2*p^3 - 9450*u*p^3*q + 1575*u*p*q^2 + 63*u^3*p + 10395*u*p^5)*dt^7 + ...
                1.0/40320*(1107*u^2*q^2 - 24570*u^2*p^2*q - 2205*u^3*p^2 + 51975*u^2*p^4 - 42525*u*p^2*q^2 + 155925*u*p^4*q + 1575*u*q^3 + 117*u^3*q - 135135*u*p^6 + u^4)*dt^8;

            g = dt - 1/6*u*dt^3 + 0.25*u*p*dt^4 + 1/120*(9*u*q - 45*u*p^2 + u^2)*dt^5 + ...
                1/360*(210*u*p^3 - 90*u*p*q - 15*u^2*p)*dt^6 + ...
                1/5040*(3150*u*p^2*q - 54*u^2*q - 225*u*q^2 - 4725*u*p^4 + 630*u^2*p^2 - u^3)*dt^7 + ...
                1/40320*(3024*u^2*p*q - 12600*u^2*p^3 - 56700*u*p^3*q + 9450*u*p*q^2 + 62370*u*p^5 + 126*u^3*p)*dt^8;
            
            fdot = -u*dt + 1.5*u*p*dt^2 + 1.0/6.0*(3*u*q - 15*u*p^2 + u^2)*dt^3 + ...
                5.0/8.0*(7*u*p^3 - 3*u*p*q - u^2*p)*dt^4 +  ...
                6.0/720.0*(630*u*p^2*q - 24*u^2*q - u^3 - 45*u*q^2 - 945*u*p^4 + 210*u^2*p^2)*dt^5 + ...
                7.0/5040*(882*u^2*p*q - 3150*u^2*p^3 - 9450*u*p^3*q + 1575*u*p*q^2 + 63*u^3*p + 10395*u*p^5)*dt^6 + ...
                8.0/40320*(1107*u^2*q^2 - 24570*u^2*p^2*q - 2205*u^3*p^2 + 51975*u^2*p^4 - 42525*u*p^2*q^2 + 155925*u*p^4*q + 1575*u*q^3 + 117*u^3*q - 135135*u*p^6 + u^4)*dt^7;

            gdot = 1 - 0.5*u*dt^2 + u*p*dt^3 + 5/120*(9*u*q - 45*u*p^2 + u^2)*dt^4 + ...
                1/60*(210*u*p^3 - 90*u*p*q - 15*u^2*p)*dt^5 + ...
                7/5040*(3150*u*p^2*q - 54*u^2*q - 225*u*q^2 - 4725*u*p^4 + 630*u^2*p^2 - u^3)*dt^6 + ...
                8/40320*(3024*u^2*p*q - 12600*u^2*p^3 - 56700*u*p^3*q + 9450*u*p*q^2 + 62370*u*p^5 + 126*u^3*p)*dt^7;

            stm(1,1) = f;
            stm(2,2) = f;
            stm(3,3) = f;
            stm(1,4) = g;
            stm(2,5) = g;
            stm(3,6) = g;
            stm(4,1) = fdot;
            stm(5,2) = fdot;
            stm(6,3) = fdot;
            stm(4,4) = gdot;
            stm(5,5) = gdot;
            stm(6,6) = gdot;
            if i == 1
                stm
            end
            x1 = stm*x;

            r(1) = x1(1);
            r(2) = x1(2);
            r(3) = x1(3);  
            dr = rans(i,:) - r;
            fprintf(outfile,'%6i x %14.7f  %14.7f  %14.7f  %14.7f  %14.7f  %14.7f dr %9.4f km\n',dt,x1,mag(dr) );
             ro(1) = x1(1);
             ro(2) = x1(2);
             ro(3) = x1(3);
             vo(1) = x1(4);
             vo(2) = x1(5);
             vo(3) = x1(6);
        end;        
        
  pause;
  
  % now test sampling 4pisr space
  ro = rorig;
  vo = vorig;
  rad = 180/pi;
  oosm = 1.0/sqrt(mu);
  sm = sqrt(mu);
  
  % loop through these
  magr = 10000.0;  % km

  
  
  % go through involved and non-involved pieces of each
  % non-involved pieces have different velocity distributions (like solar
  % panels that break off etc. still exhibit ghosting. 
  % for I1 I2, NI1, NI2
    
  for i = 0:36  % loop through lon
      lon = i*10/rad;
      for j = -9:9  % through lats (+-90)
          lat = j*10/rad;
          
          % form position vectors
          r1 = ro;
          r2 = magr*[cos(lat)*cos(lon) cos(lat)*sin(lon) sin(lat)];
          
          % loop through these
          dtsec = 7635.0;

          % multi-revs are zero for now
          tbik = zeros(3);
          [v1, v2, errorl, errorout, f, g, gdot] = lambertkfg ( r1, r2, 'l', 'd', 0, 0, dtsec, tbik, outfile );
          fdot = (f*gdot - 1.0)/g;
       
          sme    = mag(v1)^2 *0.5  - ( mu /mag(r1) );  
          hbar = cross( r1,v1 );
          magh = mag( hbar );
          transp = magh*magh / mu;
          if ( abs( sme ) > 0.00001  )
               transa= -1.0 / (2.0 *sme);  % mu
          end
   
          magr1 = mag(r1);
          magr2 = mag(r2);
          u1 = -magr1*magr2*fdot*oosm;
          u2 = magr1*(1.0 - f);
          u3 = sm*(dtsec - g);
          u4 = u1*u3 - 0.5*(u2^2 - u3^2/transa);
          sig1 = dot(r1,v1)*oosm;
          sig2 = dot(r2,v2)*oosm;
          chi = dtsec*sm/transa + sig2 - sig1;
          u5 = transa*(1/6 *chi^3 - u3);
          cb = oosm *(3.0*u5 - chi*u4 - sm*u2*dtsec);
          % get outer product by r1'*r2
          partr2wpartv1 = magr1/mu*(1.0-f)*((r2-r1)'*v1 - (v2-v1)'*r1) + cb/mu*v2'*v1 + g*eye(3);
          %partr2wpartv1;
          det(partr2wpartv1);
        
          pdf = v1/det(partr2wpartv1);
          fprintf(outfile,'lat %11.7f  lon %11.7f  dt %11.3f  v %11.7g  %11.7g  %11.7g \n', lat*rad, lon*rad, dtsec, pdf(1), pdf(2), pdf(3));
      
       end % for lat
  end % for lon          
       
        
%1999 1 15  0  0  0   -4550.425600     2220.194600    5090.354700     -4.9750060      -5.2371094      -2.1181103
%1999 1 15  0 10  0   -6497.271839    -1139.240286    2948.222193     -1.3168059      -5.5987678      -4.7836404
%1999 1 15  0 20  0   -6053.867300    -4085.026963    -283.992965     2.7252423       -3.9220995      -5.6474477
%1999 1 15  0 30  0   -3449.301021    -5580.065156   -3422.404229    5.6759204       -0.9290440      -4.5045516
%1999 1 15  0 40  0     348.954922    -5155.414419   -5387.850237    6.6101118       2.2888127       -1.8668426
%1999 1 15  0 50  0    4034.043662    -2999.082931   -5546.087947    5.3219176       4.6890137       1.3502643
%1999 1 15  1  0  0    6374.982340      155.801658   -3857.108093    2.2495089       5.5258248       4.1233447
%1999 1 15  1 10  0    6564.333364     3254.003744    -870.444256     -1.6565747      4.4918587       5.5458494
%1999 1 15  1 20  0    4477.838127     5217.614680    2411.553540     -5.1077460      1.8371158       5.0571827
%1999 1 15  1 30  0     788.147905     5303.146571    4820.098179     -6.8112637      -1.5791089      2.7049351
