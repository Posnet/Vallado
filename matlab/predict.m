%
%
%   prediction problem
%
%
      constant;

      testnum = 3;

        if testnum == 3
           p      = 8634.2349;
           ecc    =    0.002;
           incl   =   50.0/rad;
           omega  =  115.0/rad;
           argp   =   90.0/rad;
           nu     =    0.0/rad;
           arglat =    0.0/rad;
           truelon=    0.0/rad;
           lonper =    0.0/rad;
           [reci,veci] = coe2rv(p,ecc,incl,omega,argp,nu,arglat,truelon,lonper);

           reci = [6585.038266; 1568.184321; 9.116355];
           veci = [-1.1157766; 4.6316816;  6.0149576];
           aeci = [0.001;0.002;0.003];

           year = 1997;
           mon  =   4;
           day  =   1;
           hr   =  21;
           min  =  36;
           sec  =  0.0000;
           dut1 = -0.2913774;
           dat  = 32;
           xp   = -0.19108;
           yp   =  0.329624;
           lod  =  0.0;
           terms = 0;
        timezone= 0;   % this doens't really work
          end

            fprintf(1,'year %5i ',year);
            fprintf(1,'mon %4i ',mon);
            fprintf(1,'day %3i ',day);
            fprintf(1,'hr %3i:%2i:%8.6f\n',hr,min,sec );
            fprintf(1,'dut1 %8.6f s',dut1);
            fprintf(1,' dat %3i s',dat);
            fprintf(1,' xp %8.6f "',xp);
            fprintf(1,' yp %8.6f "',yp);
            fprintf(1,' lod %8.6f s\n',lod);


            % -------- convtime    - convert time from utc to all the others
%            fprintf(1,'convtime results\n');
%            fprintf(1,'ut1 %8.6f tut1 %16.12f jdut1 %18.11f\n',ut1,tut1,jdut1 );
%            fprintf(1,'utc %8.6f\n',utc );
%            fprintf(1,'tai %8.6f\n',tai );
%            fprintf(1,'tt  %8.6f ttt  %16.12f jdtt  %18.11f\n',tt,ttt,jdtt );
%            fprintf(1,'tdb %8.6f ttdb %16.12f jdtdb %18.11f\n',tdb,ttdb,jdtdb );



            % ---- perform prediction
            [jdepoch,jdepochf] = jday(year,mon,day,hr,min,sec);

            dtsec = 120.0;

            latgd = 42.38/rad;
            lon =  -71.13/rad;
            alt = 0.024; %km
            [rsecef,vsecef] = site ( latgd,lon,alt );
            fprintf(1,'site ecef %14.7f%14.7f%14.7f%14.7f%14.7f%14.7f\n',rsecef,vsecef );


            ndot = 0.0;
            nddot = 0.0;
            rho = 0.0;
            az  = 0.0;
            el  = 0.0;
            vis = 'radar sun';

            for i = 0:120
%                [reci1,veci1,error] =  kepler  ( reci,veci, i*dtsec );
%                reci = reci';
%                veci = veci';
                [reci1,veci1] =  pkepler  ( reci,veci,ndot,nddot, i*dtsec );
               if i == 106
                  fprintf(1,'reci1 %4i x %11.7f  %11.7f  %11.7f  %11.7f  %11.7f  %11.7f \n',i,reci1,veci1 );
                end;

                [ut1, tut1, jdut1,jdut1frac, utc, tai, tt, ttt, jdtt,jdttfrac, tdb, ttdb, jdtdb,jdtdbfrac ] ...
                = convtime ( year, mon, day, hr, min, sec+ i*dtsec, timezone, dut1, dat );


                % -------------------- convert eci to ecef --------------------
                a = [0;0;0];
%          reci1 = reci1';
%          veci1 = veci1';
                [recef,vecef,aecef] = eci2ecef(reci1,veci1,a,ttt,jdut1+jdut1frac,lod,xp,yp,terms);

               if i == 106
                  fprintf(1,'recef%4i x %11.7f  %11.7f  %11.7f  %11.7f  %11.7f  %11.7f \n',i,recef,vecef );
                end;

                % ------- find ecef range vector from site to satellite -------
                rhoecef  = recef - rsecef;

                % ------------- convert to sez for calculations ---------------
                [tempvec]= rot3( rhoecef, lon          );
                [rhosez ]= rot2( tempvec, halfpi-latgd );
               if i == 106
                  fprintf(1,'rhosez %4i x %11.7f  %11.7f  %11.7f \n',i,rhosez );
                end;

                [rho,az,el,drho,daz,del] = rv2razel ( reci1,veci1, latgd,lon,alt,ttt,jdut1+jdut1frac,lod,xp,yp,terms );
%                fprintf(1,'rvraz %14.7f%14.7f%14.7f%14.7f%14.7f%14.7f\n',rho,az*rad,el*rad,drho,daz*rad,del*rad );

                if az < 0.0
                    az = az + twopi;
                  end;

                if rhosez(3) > 0.0
                    [rsun,rtasc,decl] = sun ( jdtt+jdttfrac );
                    if i == 106
                      fprintf(1,'rsun%4i %11.7f  %11.7f  %11.7f \n',i,rsun );
                      fprintf(1,'rsun%4i %11.7f  %11.7f  %11.7f \n',i,rsun*149597870.0 );
                    end;
                     rsun = rsun * 149597870.0;

                    [rseci,vseci,aeci] = ecef2eci(rsecef,vsecef,a,ttt,jdut1+jdut1frac,lod,xp,yp,2);
                    if i == 106
                       fprintf(1,'rseci %4i x %11.7f  %11.7f  %11.7f  %11.7f  %11.7f  %11.7f \n',i,rseci,vseci );
                     end;
                    if dot(rsun,rseci) > 0.0
                        vis='radar sun';
                      else
                        rxr = cross(rsun,reci1);
                        magrxr = mag(rxr);
                        magr=mag(reci1);
                        magrsun=mag(rsun);
                        zet = asin( magrxr/(magrsun*magr) ) ;
                        dist = mag(reci1) * cos(zet-halfpi);
                        if i == 106
                          fprintf(1,'zet  %11.7f dist %11.7f  \n',zet*rad,dist );
                         end;
                        if dist > re
                            vis = 'visible';
                          else
                            vis = 'radar night';
                          end;
                      end;
                    else
                      vis = 'not visible';
                    end;

                [y,m,d,h,mn,s] = invjday( jdut1,jdut1frac - dut1/86400.0 );

                fprintf(1,'%5i %3i %3i %2i:%2i %6.3f %12s %11.7f  %11.7f  %11.7f  \n',y,m,d,h,mn,s,vis,rho,az*rad,el*rad);

              end;  % for




