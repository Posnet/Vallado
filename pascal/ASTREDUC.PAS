(*     ----------------------------------------------------------------      *)

                               UNIT ASTREDUC;

(*                                                                           *)
(*  This file contains Astrodynamic procedures and functions to implement    *)
(*  reduction calculations. These routines are described in Ch3.             *)
(*                                                                           *)
(*                           Companion code for                              *)
(*             Fundamentals of astrodynamics and Applications                *)
(*                                  2007                                     *)
(*                            by David Vallado                               *)
(*                                                                           *)
(*     (W) 719-573-2600, email dvallado@agi.com                              *)
(*                                                                           *)
(*     *****************************************************************     *)
(*                                                                           *)
(*  Current :                                                                *)
(*            31 May 07  David Vallado                                       *)
(*                         3rd edition baseline                              *)
(*  Changes :                                                                *)
(*            14 May 01  David Vallado                                       *)
(*                         2nd edition baseline                              *)
(*            23 Nov 87  David Vallado                                       *)
(*                         Original baseline                                 *)
(*                                                                           *)

(*     ----------------------------------------------------------------      *)

                                  INTERFACE

(*     ----------------------------------------------------------------      *)

    Uses
        AstMath,
        AstTime;
    TYPE

        DataRec = RECORD
                    JD          : EXTENDED;
                    DPsiFK5, DEpsFk5,
                    DPsi96,  DEps96   : EXTENDED;
                    PNmI        : MATRIX;
                  END;

        IAr5x106  = ARRAY[1.. 5,1..106] of INTEGER;
        RAr4x106  = ARRAY[1.. 4,1..106] of EXTENDED;
        IAr5x263  = ARRAY[1.. 5,1..263] of INTEGER;
        RAr6x263  = ARRAY[1.. 6,1..263] of EXTENDED;
        RAr6x106  = ARRAY[1.. 6,1..106] of EXTENDED;
        IAr10x112 = ARRAY[1..10,1..112] of INTEGER;
        RAr4x112  = ARRAY[1.. 4,1..112] of EXTENDED;

    VAR
        PreTime,
        NutTimeFK5, NutTimeFK5L,
        NutTime96 , NutTime96L , NutTime96p,  NutTimePNMI,  NutTimePNMII,  NutTimePnmIII,
        Tem1Time,   Tem2Time,    Tem3Time,    Tem4Time,

        { Do these as global so they can be accessed }
        { Each declaration is repeated, but commented out in the procedures }
        DEpsFK5,     DEps96,
        DPsi96P,     DEps96P,
        DPsiFK5L,    DEpsFK5L : EXTENDED;

        PrecMat,NutMat   : Matrix;

        th,tm,ts,ts1     : Word;

        outfile,outfile1 : TEXT;

{}
PROCEDURE InitNutation       ( FileN1,FileN2                         : Str64;
                               VAR IAr00                             : IAr5x106;
                               VAR RAr00                             : RAr6x106;
                               VAR IAr80                             : IAr5x106;
                               VAR RAr80                             : RAr4x106;
                               VAR IAr96                             : IAr5x263;
                               VAR RAr96                             : RAr6x263;
                               VAR pIAr96                            : IAr10x112;
                               VAR pRAr96                            : RAr4x112 );


PROCEDURE IAU2000GCRF        ( VAR rGCRF ,vGCRF                      : Vector;
                               Direction                             : Direct;
                               VAR rITRF,vITRF                       : Vector;
                               JDUT1,TTT,xp,yp                       : EXTENDED;
                               IAr00                                 : IAr5x106;
                               RAr00                                 : RAr6x106 );

PROCEDURE PRECESSION         ( VAR rJ2000,vJ2000                     : Vector;
                               Direction                             : Direct;
                               VAR rMOD,vMOD                         : Vector;
                               TTT                                   : EXTENDED );

PROCEDURE NUT80FK5           ( VAR rMOD,vMOD                         : Vector;
                               Direction                             : Direct;
                               VAR rTOD,vTOD                         : Vector;
                               VAR DPsiFK5,TrueEpsFK5,Omega          : EXTENDED;
                               TTT                                   : EXTENDED;
                               IAr80                                 : IAr5x106;
                               RAr80                                 : RAr4x106 );

PROCEDURE NUT80GCRF          ( VAR rMOD,vMOD                         : Vector;
                               Direction                             : Direct;
                               VAR rTOD,vTOD                         : Vector;
                               VAR DPsiFK5,TrueEpsFK5,Omega          : EXTENDED;
                               TTT,DDPsi,DDEps                       : EXTENDED;
                               IAr80                                 : IAr5x106;
                               RAr80                                 : RAr4x106 );

PROCEDURE NUT96FK5           ( VAR rMOD,vMOD                         : Vector;
                               Direction                             : Direct;
                               VAR rTOD,vTOD                         : Vector;
                               VAR DPsi96,TrueEps96,Omega            : EXTENDED;
                               TTT                                   : EXTENDED;
                               IAr96                                 : IAr5x263;
                               RAr96                                 : RAr6x263;
                               pIAr96                                : IAr10x112;
                               pRAr96                                : RAr4x112  );

PROCEDURE SIDEREAL           ( VAR rTOD,vTOD                         : Vector;
                               Direction                             : Direct;
                               VAR rECEF,vECEF                       : Vector;
                               JDUT1,Omega,DPsi,MeanEps,XLOD         : EXTENDED );

PROCEDURE POLARM             ( VAR rECEF,vECEF                       : Vector;
                               Direction                             : Direct;
                               VAR rITRF,vITRF                       : Vector;
                               xp,yp                                 : EXTENDED );

PROCEDURE TrueMean           ( VAR rMOD,vMOD                         : Vector;
                               Direction                             : Direct; 
                               VAR rTM,vTM                           : Vector;
                               VAR DPsiTM,TrueEpsTM                  : EXTENDED;
                               TTT                                   : EXTENDED;
                               IAr80                                 : IAr5x106;
                               RAr80                                 : RAr4x106 );

PROCEDURE FK4                ( VAR rJ2000,vJ2000                     : Vector;
                               Direction                             : Direct;
                               VAR rFK4,vFK4                         : Vector   );

{ ------------------------- Constants used in this Library ---------------------
|
|     TwoPi      : EXTENDED =     6.28318530717959;
|     OmegaEarth : EXTENDED =     0.05883359980154919;
|     Rad2Deg    : EXTENDED =    57.29577951308230;
|
 ----------------------------------------------------------------------------- }

(*     -----------------------------------------------------------------      *)

                                IMPLEMENTATION

(*     -----------------------------------------------------------------      *)
{}
{ ----------------------------------------------------------------------------
|
|                           PROCEDURE INITNUTATION
|
|  This procedure initializes the nutation matricies needed for reduction
|    calculations. The routine needs the filename of the files as input.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|                                               
|  Inputs          Description                    Range / Units
|    FileN1      - Filename for nutation terms
|    FileN2      - Filename for planetary nutation terms
|
|  Outputs       :
|                - Arrays of data for each technique
|
|  Locals        :
|    Convrt      - Changes milli-arcseconds to degrees
|
|  Coupling      :
|    None        -
|
 ----------------------------------------------------------------------------- }

PROCEDURE InitNutation       ( FileN1,FileN2                         : Str64;
                               VAR IAr00                             : IAr5x106;
                               VAR RAr00                             : RAr6x106;
                               VAR IAr80                             : IAr5x106;
                               VAR RAr80                             : RAr4x106;
                               VAR IAr96                             : IAr5x263;
                               VAR RAr96                             : RAr6x263;
                               VAR pIAr96                            : IAr10x112;
                               VAR pRAr96                            : RAr4x112  );
   VAR
     i      : INTEGER;
     Convrt : EXTENDED;
     InFile : TEXT;
   BEGIN
     ASSIGN( Infile,FileN1 );
     RESET (Infile );

     { --------- Read in 2000 IAU Theory of Nutation Terms ---------- }
     Convrt:= 0.0001/3600.0; { 0.0001" to deg } 
     READLN (Infile );  { Header }
     READLN (Infile );  { Header }
     FOR i:= 1 to 106 DO
       BEGIN
         Readln( InFile,IAr00[1,i],IAr00[2,i],IAr00[3,i],IAr00[4,i],IAr00[5,i],
                        RAr00[1,i],RAr00[2,i],RAr00[3,i],RAr00[4,i],RAr00[5,i],RAr00[6,i] );
         RAr00[1,i]:= RAr00[1,i] * Convrt;
         RAr00[2,i]:= RAr00[2,i] * Convrt;
         RAr00[3,i]:= RAr00[3,i] * Convrt;
         RAr00[4,i]:= RAr00[4,i] * Convrt;
         RAr00[5,i]:= RAr00[5,i] * Convrt;
         RAr00[6,i]:= RAr00[6,i] * Convrt;
       END;
     
     { --------- Read in 1980 IAU Theory of Nutation Terms ---------- }
     Convrt:= 0.0001/3600.0; { 0.0001" to deg } 
     READLN (Infile );  { Header }
     READLN (Infile );  { Header }
     FOR i:= 1 to 106 DO
       BEGIN
         Readln( InFile,IAr80[1,i],IAr80[2,i],IAr80[3,i],IAr80[4,i],IAr80[5,i],
                        RAr80[1,i],RAr80[2,i],RAr80[3,i],RAr80[4,i] );
         RAr80[1,i]:= RAr80[1,i] * Convrt;
         RAr80[2,i]:= RAr80[2,i] * Convrt;
         RAr80[3,i]:= RAr80[3,i] * Convrt;
         RAr80[4,i]:= RAr80[4,i] * Convrt;
       END;
     
     { --------- Read in 1996 IAU Theory of Nutation Terms ---------- }
     Convrt:= 0.000001/3600.0; { 0.000001" to deg }
     READLN (Infile );  { Header }
     READLN (Infile );  { Header }
     FOR i:= 1 to 263 DO
       BEGIN
         Readln( InFile,IAr96[1,i],IAr96[2,i],IAr96[3,i],IAr96[4,i],IAr96[5,i],
                        RAr96[1,i],RAr96[2,i],RAr96[3,i],RAr96[4,i],RAr96[5,i],RAr96[6,i] );
         RAr96[1,i]:= RAr96[1,i] * Convrt;
         RAr96[2,i]:= RAr96[2,i] * Convrt;
         RAr96[3,i]:= RAr96[3,i] * Convrt;
         RAr96[4,i]:= RAr96[4,i] * Convrt;
         RAr96[5,i]:= RAr96[5,i] * Convrt;
         RAr96[6,i]:= RAr96[6,i] * Convrt;
       END;
     CLOSE( InFile );
     
     { ----------- Read in 1996 Planetary Nutation Terms ------------ }
     Convrt:= 0.000001/3600.0; { 0.000001" to deg }
     ASSIGN( Infile,FileN2 );
     RESET (Infile );
     READLN (Infile );  { Header }
     READLN (Infile );  { Header }
     FOR i:= 1 to 112 DO
       BEGIN
         Readln( InFile,pIAr96[1,i],pIAr96[2,i],pIAr96[3,i],pIAr96[4,i],pIAr96[5,i],
                        pIAr96[6,i],pIAr96[7,i],pIAr96[8,i],pIAr96[9,i],pIAr96[10,i],
                        pRAr96[1,i],pRAr96[2,i],pRAr96[3,i],pRAr96[4,i] );
         pRAr96[1,i]:= pRAr96[1,i] * Convrt;
         pRAr96[2,i]:= pRAr96[2,i] * Convrt;
         pRAr96[3,i]:= pRAr96[3,i] * Convrt;
         pRAr96[4,i]:= pRAr96[4,i] * Convrt;
       END;

     CLOSE( InFile );
   END; { Procedure InitNutation }
{}
{}
{ ----------------------------------------------------------------------------
|
|                           PROCEDURE IAU2000GCRF
|
|  This PROCEDURE converts position and velocity vectors between the
|    Inertial Geocentric Celestial Reference Frame (GCRF), and the
|    Body Fixed, International Terrestrial Reference Frame (ITRF).
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    rGCRF       - Initial GCRF ECI Position      ER
|    vGCRF       - Initial GCRF ECI Velocity      ER/TU
|    TTT         - Julian Centuries of TT         centuries
|    Direction   - Which set of vars to output    FROM  TOO
|
|  Outputs       :
|    vITRF       - Initial ITRF Velocity          ER/TU
|    rITRF       - Initial ITRF Position          ER
|
|  Locals        :
|    TTT2        - TTT squared
|    TTT3        - TTT cubed
|    Zeta        - PRECESSION ANGLE               rad
|    z           - PRECESSION ANGLE               rad
|    Theta       - PRECESSION ANGLE               rad
|
|  Coupling      :
|    ROT2        - Rotation about the second axis
|    ROT3        - Rotation about the third axis
|
|  References    :
|    Vallado       2007, 228
|
  ---------------------------------------------------------------------------- }

PROCEDURE IAU2000GCRF        ( VAR rGCRF ,vGCRF                      : Vector;
                               Direction                             : Direct;
                               VAR rITRF,vITRF                       : Vector;
                               JDUT1,TTT,xp,yp                       : EXTENDED;
                               IAr00                                 : IAr5x106;
                               RAr00                                 : RAr6x106 );
   CONST
     Deg2Rad    : EXTENDED =     0.01745329251994;
     OmegaEarth : EXTENDED =     0.05883359980154919;
     RadiusEarthKM : EXTENDED =    6378.1363;
     VKmPerSec     : EXTENDED =       7.905366149846;
   VAR
     v3a,r1,r2,r3,v1,v2,v3,temp, temp1 : Vector;
     zeta, z, theta, TTT2, TTT3 : EXTENDED;

     i, deg, MIN : INTEGER;
     LonMer,LonVen,LonEar,LonMar,LonJup,LonSat,PrecRate,SA,
     DeltaPsi00, DeltaEps00,
     x,y,s,a,aa,ac,accdispl,deltapsi,deltaeps,
     Eps,TrueEps,
     SEC, Tempval, TTT4, rr, l, l1, f, d, Omega : EXTENDED;
     PN : ARRAY[1..3,1..3] of Extended;

   BEGIN
     { ------- Determine coefficients for IAU 2000(xx) NUTATION Theory -------- }
     TTT2:= TTT*TTT;
     TTT3:= TTT2*TTT;
     TTT4:= TTT3*TTT;
{GetTime( th,tm,ts,ts1);
Tem1Time:= th*3600 + tm*60 + ts + ts1/100;}

     Eps := 23.439291 - 0.0130042*TTT - 0.000000164*TTT2 + 0.000000504*TTT3;
     Eps := MODFUNC( Eps,360.0 );
     Eps := Eps * Deg2Rad;

     { -------------- Evaluate Brown's fundamental arguments ---------------- }
     rr   := 360.0; {deg}
     l    :=  134.96340251 + (1325*rr  + 198.8675605)*TTT + 0.0088553 *TTT2
                           + 0.000014343*TTT3 - 0.00000006797 * TTT4;
     l1   :=  357.52910918 + (  99*rr  + 359.0502911)*TTT - 0.0001537 *TTT2
                           - 0.000000038*TTT3 - 0.00000000319 * TTT4;
     F    :=   93.27209062 + (1342*rr  +  82.0174577)*TTT - 0.0035420 *TTT2
                           + 0.000000288*TTT3 + 0.00000000116 * TTT4;
     D    :=  297.85019547 + (1236*rr  + 307.1114469)*TTT - 0.0017696 *TTT2
                           + 0.000001831*TTT3 - 0.00000000880 * TTT4;
     Omega:=  125.04455501 - (   5*rr  + 134.1361851)*TTT + 0.0020756 *TTT2
                           + 0.000002139*TTT3 - 0.00000001650 * TTT4;
     l    := MODFUNC( l,360.0 )     * Deg2Rad;
     l1   := MODFUNC( l1,360.0 )    * Deg2Rad;
     F    := MODFUNC( F,360.0 )     * Deg2Rad;
     D    := MODFUNC( D,360.0 )     * Deg2Rad;
     Omega:= MODFUNC( Omega,360.0 ) * Deg2Rad;

     DeltaPsi00:= 0.0;
     DeltaEps00:= 0.0;
     FOR i:= 106 downto 1 DO
       BEGIN
         Tempval:= IAr00[1,i]*l + IAr00[2,i]*l1 + IAr00[3,i]*F + IAr00[4,i]*D + IAr00[5,i]*Omega;
         DeltaPsi00:= DeltaPsi00 + (RAr00[1,i]+RAr00[2,i]*TTT) * SIN( TempVal )
                                         + RAr00[5,i] * TTT * COS( TempVal );
         DeltaEps00:= DeltaEps00 + (RAr00[3,i]+RAr00[4,i]*TTT) * COS( TempVal )
                                         + RAr00[6,i] * TTT * SIN( TempVal );
       END;
     TrueEps:= Eps + DeltaEps00;

     { ------------------------ Find Stellar Angle ------------------- }
     SA:= 2.0*Pi*(0.779057273264 + 1.00273781191135448*(JDUT1 - 2451545.0)); {rad}
     TempVal:= SA;
     SA:= MODFUNC( SA,2.0*Pi );  {Rad}

     X:= 2004.3109*TTT - 0.42665*TTT2 - 0.198656*TTT3 + 0.0000140*TTT4
         + 0.00006*TTT2*COS(Omega) 
         + 0.00204*TTT2*SIN(Omega) + 0.00016*TTT2*COS( 2*(F - D + Omega) );
     X:= X * Deg2Rad / 3600.0 + SIN(TrueEps)*DeltaPsi00; { " to rad }
     Y:= -0.00013 - 22.40992*TTT2 + 0.001836*TTT3 + 0.0011130*TTT4
         - 0.00231*TTT2*COS(Omega) - 0.00014*TTT2*COS( 2*(F - D + Omega) );
     Y:= Y * Deg2Rad / 3600.0 + DeltaEps00; { " to rad }

     a:= 0.5 + 0.125*(X*X+Y*Y);  {"}
     s:= -X*Y*0.5 + 0.00385*TTT - 0.07259*TTT3 - 0.00264*SIN(Omega)
         - 0.00006*SIN(2*Omega) + 0.00074*TTT2*SIN(Omega)
         + 0.0006*TTT2*SIN( 2*(F - D + Omega) );

     a:= a * Deg2Rad / 3600.0; { " to rad }
     s:= s * Deg2Rad / 3600.0; { " to rad }

     aa:= 0.12; { " }
     ac:= 0.26;
     AccDispl:= 0.0015*(ac*ac/1.2 + aa*aa)*TTT* Deg2Rad / 3600.0;  { " to rad }

     PN[1,1]:= 1.0 - a*X*X;
     PN[1,2]:= -a*X*Y;
     PN[1,3]:= X;
     PN[2,1]:= PN[1,2];
     PN[2,2]:= 1.0 - a*Y*Y;
     PN[2,3]:= Y;
     PN[3,1]:= -X;
     PN[3,2]:= -Y;
     PN[3,3]:= 1.0 - a*(X*X + Y*Y);

     IF Direction = TOO THEN
       BEGIN
         r1[1]:= PN[1,1]*rGCRF[1] + PN[1,2]*rGCRF[2] + PN[1,3]*rGCRF[3];
         r1[2]:= PN[2,1]*rGCRF[1] + PN[2,2]*rGCRF[2] + PN[2,3]*rGCRF[3];
         r1[3]:= PN[3,1]*rGCRF[1] + PN[3,2]*rGCRF[2] + PN[3,3]*rGCRF[3];
         MAG(r1);
         ROT3( r1    ,    S     , r2    );
         ROT3( r2    ,   SA     , r3    );
         ROT3( r3    , AccDispl , Temp  );
         ROT1( Temp  ,   -yp    , Temp1 );
         ROT2( Temp1 ,   -xp    , rITRF );

         v1[1]:= PN[1,1]*vGCRF[1] + PN[1,2]*vGCRF[2] + PN[1,3]*vGCRF[3];
         v1[2]:= PN[2,1]*vGCRF[1] + PN[2,2]*vGCRF[2] + PN[2,3]*vGCRF[3];
         v1[3]:= PN[3,1]*vGCRF[1] + PN[3,2]*vGCRF[2] + PN[3,3]*vGCRF[3];
         MAG(v1);
         ROT3( v1    ,    S     , v2    );
         ROT3( v2    ,   SA     , v3a   );
         v3[1]:= v3a[1] + r3[2]*OmegaEarth;
         v3[2]:= v3a[2] - r3[1]*OmegaEarth;
         v3[3]:= v3a[3];
         MAG( v3 );
         ROT3( v3    , AccDispl , Temp  );
         ROT1( Temp  ,   -yp    , Temp1 );
         ROT2( Temp1 ,   -xp    , vITRF );
       END
       ELSE
       BEGIN
         ROT2( ritrf ,    xp    , Temp  );
         ROT1( temp  ,    yp    , Temp1 );
         ROT3( Temp1 ,-AccDispl , r3    );
         ROT3( r3    ,  -SA     , r2    );
         ROT3( r2    ,   -S     , r1    );
         rGCRF[1]:= PN[1,1]*r1[1] + PN[2,1]*r1[2] + PN[3,1]*r1[3];
         rGCRF[2]:= PN[1,2]*r1[1] + PN[2,2]*r1[2] + PN[3,2]*r1[3];
         rGCRF[3]:= PN[1,3]*r1[1] + PN[2,3]*r1[2] + PN[3,3]*r1[3];
         MAG(rGCRF);

         ROT2( vitrf ,    xp    , Temp  );
         ROT1( temp  ,    yp    , Temp1 );
         ROT3( Temp1 ,-AccDispl , v3    );
         v3a:= v3;
         v3a[1]:= v3[1] - r3[2]*OmegaEarth;
         v3a[2]:= v3[2] + r3[1]*OmegaEarth;
         MAG( v3a );
         ROT3( v3a   ,  -SA     , v2   );
         ROT3( v2    ,   -S     , v1    );
         vGCRF[1]:= PN[1,1]*v1[1] + PN[2,1]*v1[2] + PN[3,1]*v1[3];
         vGCRF[2]:= PN[1,2]*v1[1] + PN[2,2]*v1[2] + PN[3,2]*v1[3];
         vGCRF[3]:= PN[1,3]*v1[1] + PN[2,3]*v1[2] + PN[3,3]*v1[3];
         MAG(vGCRF);
       END;

     IF Show = 'I' THEN
       BEGIN
         WriteLn( FileOut, '------  IAU2000 ----------' );
         WriteLn( FileOut, 'Input vars --------- ');
         WriteLn( FileOut, 'JDUT1 ',JDUT1:18:12,'s' );
         WriteLn( FileOut, 'TTT   ',TTT:14:9,'s' );
         WriteLn( FileOut, 'Intermediate vars -- ');
         WriteLn( FileOut, 'l       ',l/Deg2Rad:18:12    ,'ø',l:18:12 );
         WriteLn( FileOut, 'l1      ',l1/Deg2Rad:18:12   ,'ø',l1:18:12 );
         WriteLn( FileOut, 'f       ',f/Deg2Rad:18:12    ,'ø',f:18:12 );
         WriteLn( FileOut, 'd       ',d/Deg2Rad:18:12    ,'ø',d:18:12 );
         WriteLn( FileOut, 'omega   ',omega/Deg2Rad:18:12,'ø',omega:18:12 );
         DMS_RAD(deg,MIN,SEC,FROM, eps );
         WriteLn( FileOut, 'DeltaPsi00 ',DeltaPsi00/Deg2Rad:18:12      ,'ø',DeltaPsi00:18:12 );
         WriteLn( FileOut, 'Eps     ',Eps/Deg2Rad:18:12  ,'ø',deg:3,MIN:3,SEC:9:5,Eps:18:12 );
         WriteLn( FileOut, 'DeltaEps00 ',DeltaEps00/Deg2Rad:18:12      ,'ø',DeltaEps00:18:12 );
         WriteLn( FileOut, 'TrueEps ',TrueEps/Deg2Rad:18:12  ,'ø' );
         WriteLn( FileOut, 'x       ',x/Deg2Rad:18:12    ,'ø',x:18:12 );
         WriteLn( FileOut, 'y       ',y/Deg2Rad:18:12    ,'ø',y:18:12 );
         WriteLn( FileOut, 'a       ',a/Deg2Rad:18:12    ,'ø',a:18:12 );
         WriteLn( FileOut, 's       ',s/Deg2Rad:18:12    ,'ø',s:18:12 );

         WriteLn( FileOut, 'r2 aftSA',r2[1]*radiusearthkm:18:12,r2[2]*radiusearthkm:18:12,
                           r2[3]*radiusearthkm:18:12 );
         WriteLn( FileOut, 'V2      ',v2[1]*vkmpersec:18:12,v2[2]*vkmpersec:18:12,
                                                    v2[3]*vkmpersec:18:12 );
         WriteLn( FileOut, 'SA      ',SA/Deg2Rad:18:12,SA:18:12 );
         WriteLn( FileOut, 'SA      ',TempVal/Deg2Rad:18:12,TempVal:18:12 );

         WriteLn( FileOut, 'R3 aftPM',r3[1]*radiusearthkm:18:12,r3[2]*radiusearthkm:18:12,
                           r3[3]*radiusearthkm:18:12 );
         WriteLn( FileOut, 'V3      ',v3[1]*vkmpersec:18:12,v3[2]*vkmpersec:18:12,
                                                    v3[3]*vkmpersec:18:12 );
         WriteLn( FileOut, 'sp      ',accdispl/Deg2Rad:18:12    ,'ø',accdispl:18:12 );
         WriteLn( FileOut, 'xp ',xp:12:9,' yp ',yp:12:9,' rad' );
         WriteLn( FileOut, 'xp ',xp*3600.0*57.29577951308:12:9,
                           ' yp ',yp*3600.0*57.29577951308:12:9,' "' );
         WriteLn( FileOut,'Output vars -------- ');
       END;
   END;  { PROCEDURE IAU2000GCRF }
{}
{ ----------------------------------------------------------------------------
|
|                           PROCEDURE PRECESSION
|
|  This PROCEDURE converts position and velocity vectors between the
|    J2000 epoch (FK5 ECI) frame (Mean Equator Mean Equinox of J2000) and the
|    Mean equator Mean equinox of date (MOD).
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    rJ2000      - Initial J2000 ECI Position     ER
|    vJ2000      - Initial J2000 ECI Velocity     ER/TU
|    TTT         - Julian Centuries of TT         centuries
|    Direction   - Which set of vars to output    FROM  TOO
|
|  Outputs       :
|    rMOD        - Position vector of date
|                    Mean Equator, Mean Equinox   ER
|    vMOD        - Velocity vector of date
|                    Mean Equator, Mean Equinox   ER/TU
|
|  Locals        :
|    TTT2        - TTT squared
|    TTT3        - TTT cubed
|    Zeta        - PRECESSION ANGLE               rad
|    z           - PRECESSION ANGLE               rad
|    Theta       - PRECESSION ANGLE               rad
|
|  Coupling      :
|    ROT2        - Rotation about the second axis
|    ROT3        - Rotation about the third axis
|
|  References    :
|    Vallado       2007, 228
|
  ---------------------------------------------------------------------------- }

PROCEDURE PRECESSION         ( VAR rJ2000,vJ2000                     : Vector;
                               Direction                             : Direct;
                               VAR rMOD,vMOD                         : Vector;
                               TTT                                   : EXTENDED );
   CONST
     Deg2Rad    : EXTENDED =     Pi/180.0;
   VAR
     temp, temp1 : Vector;
     zeta, z, theta, TTT2, TTT3 : EXTENDED;
     M1,m2,m3,mx : Matrix;
   BEGIN

     { ---------------------- 1976 IAU PRECESSION angles --------------------- }
     TTT2:= TTT * TTT;
     TTT3:= TTT2 * TTT;
     Zeta := 2306.2181*TTT + 0.30188*TTT2 + 0.017998*TTT3; { " }
     Theta:= 2004.3109*TTT - 0.42665*TTT2 - 0.041833*TTT3;
     z    := 2306.2181*TTT + 1.09468*TTT2 + 0.018203*TTT3;

     Zeta := Zeta  * Deg2Rad / 3600.0;  { rad }
     Theta:= Theta * Deg2Rad / 3600.0;
     Z    := Z     * Deg2Rad / 3600.0;

     { ---------------------- Perform rotations --------------------- }
     IF Direction = TOO THEN
       BEGIN
         ROT3( rJ2000, -Zeta, Temp1 );
         ROT2( Temp1 , Theta, Temp  );
         ROT3( Temp  ,  -z  , rMOD  );
         ROT3( vJ2000, -Zeta, Temp1 );
         ROT2( Temp1 , Theta, Temp  );
         ROT3( Temp  ,  -z  , vMOD  );
       END
       ELSE
       BEGIN
         ROT3( rMOD  ,   z  , Temp   );
         ROT2( Temp  ,-Theta, Temp1  );
         ROT3( Temp1 ,  Zeta, rJ2000 );
         ROT3( vMOD  ,   z  , Temp   );
         ROT2( Temp  ,-Theta, Temp1  );
         ROT3( Temp1 ,  Zeta, vJ2000 );
       END;

     IF Show = 'I' THEN
       BEGIN
         WriteLn(fileout);
         WriteLn( FileOut, '------ PRECESSION Rotation Data ----------' );
         WriteLn( FileOut, 'Input vars --------- ');
         WriteLn( FileOut, 'TTT     ',TTT:18:12 );
         WriteLn( FileOut, 'Intermediate vars -- ');
         WriteLn( FileOut, 'Output vars -------- ');
         WriteLn( FileOut, 'Zeta    ',zeta/Deg2Rad:18:12 ,'ø',zeta:18:12 );
         WriteLn( FileOut, 'Z       ',z/Deg2Rad:18:12    ,'ø',z:18:12 );
         WriteLn( FileOut, 'Theta   ',Theta/Deg2Rad:18:12,'ø',theta:18:12 );
       END;
   END;  { PROCEDURE PRECESSION }
{}
{ ----------------------------------------------------------------------------
|
|                           PROCEDURE NUT80FK5
|
|  This PROCEDURE converts position and velocity vectors between the
|    Mean Equator Mean Equinox of date (MOD) and the True equator True equinox
|    of date (TOD).  The results take into account the effects of NUTATION using
|    the 1980 IAU Theory of Nutation.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    rMOD        - Position vector of date
|                    Mean Equator, Mean Equinox   ER
|    vMOD        - Velocity vector of date
|                    Mean Equator, Mean Equinox   ER/TU
|    TTT         - Julian Centuries of TDB
|    iAr80       - Array of data coefficients
|    rAr80       - Array of data coefficients
|    Direction   - Which set of vars to output    FROM  TOO
|
|  Outputs       :
|    rTOD        - Position vector of date
|                    True Equator, True Equinox   ER
|    vTOD        - Velocity vector of date
|                    True Equator, True Equinox   ER/TU
|    DPsiFK5     - NUTATION ANGLE                 rad
|    TrueEpsFK5  - True obliquity of the ecliptic rad
|    Omega       -                                rad
|
|  Locals        :
|    TTT2        - TTT squared
|    TTT3        - TTT cubed
|    TTT4        - TTT to the fourth
|    Eps         - Mean obliquity of the ecliptic rad
|    l           -                                rad
|    ll          -                                rad
|    F           -                                rad
|    D           -                                rad
|
|  Coupling      :
|    ROT2        - Rotation about the second axis
|    ROT3        - Rotation about the third axis
|    MODFUNC     -
|
|  References    :
|    Vallado       2007, 228
|
  ---------------------------------------------------------------------------- }

PROCEDURE NUT80FK5           ( VAR rMOD,vMOD                         : Vector;
                               Direction                             : Direct;
                               VAR rTOD,vTOD                         : Vector;
                               VAR DPsiFK5,TrueEpsFK5,Omega          : EXTENDED;
                               TTT                                   : EXTENDED;
                               IAr80                                 : IAr5x106;
                               RAr80                                 : RAr4x106 );
   CONST
     Deg2Rad    : EXTENDED =     Pi/180.0;
   VAR
     M1,m2,m3,mx : Matrix;
     Temp, temp1 : Vector;
     i, deg, MIN : INTEGER;
{     DEpsFK5,}
     Dp, De,
     TTT4, SEC, Tempval, TTT2, TTT3, eps, rr, l, l1, f, d : EXTENDED;
   BEGIN
     { ------- Determine coefficients for IAU 1980 NUTATION Theory ---------- }
     TTT2:= TTT*TTT;
     TTT3:= TTT2*TTT;
     TTT4:= TTT2*TTT2;
     Eps := - 46.8150*TTT - 0.00059*TTT2 + 0.001813*TTT3 + 84381.448; { " }
     Eps := MODFUNC( Eps/3600.0,360.0 );  { deg }
     Eps := Eps * Deg2Rad;  { rad }

{     ---- Old values ----
|     rr   := 360.0;
|     l    :=  134.9629814 + (1325*rr  + 198.8673981)*TTT + 0.0086972 *TTT2 + 0.00001778*TTT3;
|     l1   :=  357.5277233 + (  99*rr  + 359.05034  )*TTT - 0.00016028*TTT2 - 0.00000333*TTT3;
|     F    :=   93.2719103 + (1342*rr  +  82.0175381)*TTT - 0.0036825 *TTT2 + 0.00000306*TTT3;
|     D    :=  297.8503631 + (1236*rr  + 307.111480 )*TTT - 0.00191417*TTT2 + 0.00000528*TTT3;
|     Omega:=  125.0445222 - (   5*rr  + 134.1362608)*TTT + 0.0020708 *TTT2 + 0.00000222*TTT3;
}
     l    :=  134.96340251 + ( 1717915923.2178*TTT + 31.8792*TTT2
                               + 0.051635*TTT3 - 0.00024470*TTT4 ) / 3600.0; { deg and ", deg result }
     l1   :=  357.52910918 + (  129596581.0481*TTT - 0.5532*TTT2
                               - 0.000136*TTT3 - 0.00001149*TTT4 ) / 3600.0;
     F    :=   93.27209062 + ( 1739527262.8478*TTT - 12.7512*TTT2
                               + 0.001037*TTT3 + 0.00000417*TTT4 ) / 3600.0;
     D    :=  297.85019547 + ( 1602961601.2090*TTT - 6.3706*TTT2
                               + 0.006593*TTT3 - 0.00003169*TTT4 ) / 3600.0;
     Omega:=  125.04455501 + (   -6962890.2665*TTT + 7.4722*TTT2
                               + 0.007702*TTT3 - 0.00005939*TTT4 ) / 3600.0;

     l    := MODFUNC( l,360.0 )     * Deg2Rad; { rad }
     l1   := MODFUNC( l1,360.0 )    * Deg2Rad;
     F    := MODFUNC( F,360.0 )     * Deg2Rad;
     D    := MODFUNC( D,360.0 )     * Deg2Rad;
     Omega:= MODFUNC( Omega,360.0 ) * Deg2Rad;

     DPsiFK5 := 0.0;
     DPsiFK5L:= 0.0;
     DEpsFK5 := 0.0;
     DEpsFK5L:= 0.0;

     FOR i:= 106 downto 1 DO
       BEGIN
         Tempval:= IAr80[1,i]*l + IAr80[2,i]*l1 + IAr80[3,i]*F + IAr80[4,i]*D + IAr80[5,i]*Omega;
         DPsiFK5:= DPsiFK5 + (RAr80[1,i]+RAr80[2,i]*TTT) * SIN( TempVal );
         DEpsFK5:= DEpsFK5 + (RAr80[3,i]+RAr80[4,i]*TTT) * COS( TempVal );

         IF i <= 4 THEN
           BEGIN
             DPsiFK5L:= DPsiFK5L + (RAr80[1,i]+RAr80[2,i]*TTT) * SIN( TempVal );
             DEpsFK5L:= DEpsFK5L + (RAr80[3,i]+RAr80[4,i]*TTT) * COS( TempVal );
           END;
       END;

     { --------------- Find NUTATION Parameters --------------------- }
     Dp:= DPsiFK5;
     De:= DEpsFK5;
     DPsiFK5:= MODFUNC( DPsiFK5,360.0 ) * Deg2Rad; { rad }
     DEpsFK5:= MODFUNC( DEpsFK5,360.0 ) * Deg2Rad;
     TrueEpsFK5:= Eps + DEpsFK5;

     IF Direction = TOO THEN
       BEGIN
         ROT1( rMOD ,     Eps    , Temp  );
         ROT3( Temp , -DPsiFK5   , Temp1 );
         ROT1( Temp1, -TrueEpsFK5, rTOD  );
         ROT1( vMOD ,     Eps    , Temp  );
         ROT3( Temp , -DPsiFK5   , Temp1 );
         ROT1( Temp1, -TrueEpsFK5, vTOD  );

       END
       ELSE
       BEGIN
         ROT1( rTOD ,  TrueEpsFK5, Temp  );
         ROT3( Temp ,  DPsiFK5   , Temp1 );
         ROT1( Temp1,    -Eps    , rMOD  );
         ROT1( vTOD ,  TrueEpsFK5, Temp  );
         ROT3( Temp ,  DPsiFK5   , Temp1 );
         ROT1( Temp1,    -Eps    , vMOD  );
       END;

     IF Show = 'I' THEN
       BEGIN
         WriteLn( FileOut, '------  80 FK5 NUTATION ----------' );
         WriteLn( FileOut, 'Input vars --------- ');
         WriteLn( FileOut, 'TTT     ',TTT:18:12 );
         WriteLn( FileOut, 'Intermediate vars -- ');
         WriteLn( FileOut, 'l       ',l/Deg2Rad:18:12    ,'ø',l:18:12 );
         WriteLn( FileOut, 'l1      ',l1/Deg2Rad:18:12   ,'ø',l1:18:12 );
         WriteLn( FileOut, 'f       ',f/Deg2Rad:18:12    ,'ø',f:18:12 );
         WriteLn( FileOut, 'd       ',d/Deg2Rad:18:12    ,'ø',d:18:12 );
         DMS_RAD(deg,MIN,SEC,FROM, eps );
         WriteLn( FileOut, 'Eps     ',Eps/Deg2Rad:18:12  ,'ø',deg:3,MIN:3,SEC:9:5,Eps:18:12 );
         WriteLn( FileOut, 'DEpsFK5 ',DEpsFK5/Deg2Rad:18:12      ,'ø',DEpsFK5:18:12 );
         WriteLn( FileOut, 'Output vars -------- ');
         WriteLn( FileOut, 'DPsiFK5 ',DPsiFK5/Deg2Rad:18:12      ,'ø',DPsiFK5:18:12 );
         DMS_RAD(deg,MIN,SEC,FROM, TrueepsFK5 );
         WriteLn( FileOut, 'TrueEpsFK5 ',TrueEpsFK5/Deg2Rad:18:12,'ø',deg:3,MIN:3,SEC:9:5 );
         WriteLn( FileOut, 'omega   ',omega/Deg2Rad:18:12,'ø',omega:18:12 );
       END;
   END;  { PROCEDURE NUT80FK5 }
{}
{ ----------------------------------------------------------------------------
|
|                           PROCEDURE NUT80GCRF
|
|  This PROCEDURE converts position and velocity vectors between the GCRF
|    Mean Equator Mean Equinox of date (MOD) and the True equator True equinox
|    of date (TOD).  The results take into account the effects of NUTATION using
|    the 1980 IAU Theory of Nutation.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    rMOD        - Position vector of date
|                    Mean Equator, Mean Equinox   ER
|    vMOD        - Velocity vector of date
|                    Mean Equator, Mean Equinox   ER/TU
|    TTT         - Julian Centuries of TDB
|    DdPsi       - Delta NUTATION ANGLE            "
|    DdEps       - Delta obliquity of the ecliptic "
|    iAr80       - Array of data coefficients
|    rAr80       - Array of data coefficients
|    Direction   - Which set of vars to output    FROM  TOO
|
|  Outputs       :
|    rTOD        - Position vector of date
|                    True Equator, True Equinox   ER
|    vTOD        - Velocity vector of date
|                    True Equator, True Equinox   ER/TU
|    DPsiFK5     - NUTATION ANGLE                 rad
|    TrueEpsFK5  - True obliquity of the ecliptic rad
|    Omega       -                                rad
|
|  Locals        :
|    TTT2        - TTT squared
|    TTT3        - TTT cubed
|    TTT4        - TTT to the fourth
|    Eps         - Mean obliquity of the ecliptic rad
|    l           -                                rad
|    ll          -                                rad
|    F           -                                rad
|    D           -                                rad
|
|  Coupling      :
|    ROT2        - Rotation about the second axis
|    ROT3        - Rotation about the third axis
|    MODFUNC     -
|
|  References    :
|    Vallado       2007, 228
|
  ---------------------------------------------------------------------------- }

PROCEDURE NUT80GCRF          ( VAR rMOD,vMOD                         : Vector;
                               Direction                             : Direct;
                               VAR rTOD,vTOD                         : Vector;
                               VAR DPsiFK5,TrueEpsFK5,Omega          : EXTENDED;
                               TTT,DDPsi,DDEps                       : EXTENDED;
                               IAr80                                 : IAr5x106;
                               RAr80                                 : RAr4x106 );
   CONST
     Deg2Rad    : EXTENDED =     Pi/180.0;
   VAR
     Temp, temp1 : Vector;
     i, deg, MIN : INTEGER;
     Dp, De,
     TTT4, SEC, Tempval, TTT2, TTT3, eps, rr, l, l1, f, d : EXTENDED;
   BEGIN
     { ------- Determine coefficients for IAU 1980 NUTATION Theory ---------- }
     TTT2:= TTT*TTT;
     TTT3:= TTT2*TTT;
     TTT4:= TTT2*TTT2;
     Eps := - 46.8150*TTT - 0.00059*TTT2 + 0.001813*TTT3 + 84381.448; { " }
     Eps := MODFUNC( Eps/3600.0,360.0 );  { deg }
     Eps := Eps * Deg2Rad;  { rad }

     l    :=  134.96340251 + ( 1717915923.2178*TTT + 31.8792*TTT2
                               + 0.051635*TTT3 - 0.00024470*TTT4 ) / 3600.0; { deg and ", deg result }
     l1   :=  357.52910918 + (  129596581.0481*TTT - 0.5532*TTT2
                               - 0.000136*TTT3 - 0.00001149*TTT4 ) / 3600.0;
     F    :=   93.27209062 + ( 1739527262.8478*TTT - 12.7512*TTT2
                               + 0.001037*TTT3 + 0.00000417*TTT4 ) / 3600.0;
     D    :=  297.85019547 + ( 1602961601.2090*TTT - 6.3706*TTT2
                               + 0.006593*TTT3 - 0.00003169*TTT4 ) / 3600.0;
     Omega:=  125.04455501 + (   -6962890.2665*TTT + 7.4722*TTT2
                               + 0.007702*TTT3 - 0.00005939*TTT4 ) / 3600.0;

     l    := MODFUNC( l,360.0 )     * Deg2Rad; { rad }
     l1   := MODFUNC( l1,360.0 )    * Deg2Rad;
     F    := MODFUNC( F,360.0 )     * Deg2Rad;
     D    := MODFUNC( D,360.0 )     * Deg2Rad;
     Omega:= MODFUNC( Omega,360.0 ) * Deg2Rad;

     DPsiFK5 := 0.0;
     DEpsFK5 := 0.0;

     FOR i:= 106 downto 1 DO
       BEGIN
         Tempval:= IAr80[1,i]*l + IAr80[2,i]*l1 + IAr80[3,i]*F + IAr80[4,i]*D + IAr80[5,i]*Omega;
         DPsiFK5:= DPsiFK5 + (RAr80[1,i]+RAr80[2,i]*TTT) * SIN( TempVal );
         DEpsFK5:= DEpsFK5 + (RAr80[3,i]+RAr80[4,i]*TTT) * COS( TempVal );
       END;

     { --------------- Find NUTATION Parameters --------------------- }
     Dp:= DPsiFK5;
     De:= DEpsFK5;
     DPsiFK5:= DPsiFK5 + DDPSi/3600.0; { deg }
     DEpsFK5:= DEpsFK5 + DDEps/3600.0;
     DPsiFK5:= MODFUNC( DPsiFK5,360.0 ) * Deg2Rad; { rad }
     DEpsFK5:= MODFUNC( DEpsFK5,360.0 ) * Deg2Rad;
     TrueEpsFK5:= Eps + DEpsFK5;

     IF Direction = TOO THEN
       BEGIN
         ROT1( rMOD ,     Eps    , Temp  );
         ROT3( Temp , -DPsiFK5   , Temp1 );
         ROT1( Temp1, -TrueEpsFK5, rTOD  );
         ROT1( vMOD ,     Eps    , Temp  );
         ROT3( Temp , -DPsiFK5   , Temp1 );
         ROT1( Temp1, -TrueEpsFK5, vTOD  );
       END
       ELSE
       BEGIN
         ROT1( rTOD ,  TrueEpsFK5, Temp  );
         ROT3( Temp ,  DPsiFK5   , Temp1 );
         ROT1( Temp1,    -Eps    , rMOD  );
         ROT1( vTOD ,  TrueEpsFK5, Temp  );
         ROT3( Temp ,  DPsiFK5   , Temp1 );
         ROT1( Temp1,    -Eps    , vMOD  );
       END;

     IF Show = 'I' THEN
       BEGIN
         WriteLn( FileOut, '------  80 GCRF NUTATION ----------' );
         WriteLn( FileOut, 'Input vars --------- ');
         WriteLn( FileOut, 'TTT     ',TTT:18:12 );
         WriteLn( FileOut, 'DDPsi   ',DDPsi:18:12 );
         WriteLn( FileOut, 'DDEps   ',DDEps:12,'"' );
         WriteLn( FileOut, 'Intermediate vars -- ');
         WriteLn( FileOut, 'l       ',l/Deg2Rad:18:12    ,'ø',l:18:12 );
         WriteLn( FileOut, 'l1      ',l1/Deg2Rad:18:12   ,'ø',l1:18:12 );
         WriteLn( FileOut, 'f       ',f/Deg2Rad:18:12    ,'ø',f:18:12 );
         WriteLn( FileOut, 'd       ',d/Deg2Rad:18:12    ,'ø',d:18:12 );
         DMS_RAD(deg,MIN,SEC,FROM, eps );
         WriteLn( FileOut, 'Eps     ',Eps/Deg2Rad:18:12  ,'ø',deg:3,MIN:3,SEC:9:5,Eps:18:12 );
         WriteLn( FileOut, 'DPsi106 ',Dp:18:12,'ø', Dp*Deg2Rad:18:14 );
         WriteLn( FileOut, 'DEps106 ',De:18:12,'ø',De*Deg2Rad:18:14 );
         WriteLn( FileOut, 'DEpsFK5 ',DEpsFK5/Deg2Rad:18:12      ,'ø',DEpsFK5:18:12 );
         WriteLn( FileOut, 'Output vars -------- ');
         WriteLn( FileOut, 'DPsiFK5 ',DPsiFK5/Deg2Rad:18:12      ,'ø',DPsiFK5:18:12 );
         DMS_RAD(deg,MIN,SEC,FROM, TrueepsFK5 );
         WriteLn( FileOut, 'TrueEpsFK5 ',TrueEpsFK5/Deg2Rad:18:12,'ø',deg:3,MIN:3,SEC:9:5 );
         WriteLn( FileOut, 'omega   ',omega/Deg2Rad:18:12,'ø',omega:18:12 );
       END;
   END;  { PROCEDURE NUT80GCRF }
{}
{ ----------------------------------------------------------------------------
|
|                           PROCEDURE NUT96FK5
|
|  This PROCEDURE converts position and velocity vectors between the
|    Mean Equator Mean Equinox of date (MOD) and the True equator True equinox
|    of date (TOD).  The results take into account the effects of NUTATION using
|    the 1996 IAU Theory of Nutation.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    rMOD        - Position vector of date
|                    Mean Equator, Mean Equinox   ER
|    vMOD        - Velocity vector of date
|                    Mean Equator, Mean Equinox   ER/TU
|    TTT         - Julian Centuries of TDB
|    iAr96       - Array of data coefficients
|    rAr96       - Array of data coefficients
|    piAr96      - Array of data coefficients planetary
|    prAr96      - Array of data coefficients
|    Direction   - Which set of vars to output    FROM  TOO
|
|  Outputs       :
|    rTOD        - Position vector of date
|                    True Equator, True Equinox   ER
|    vTOD        - Velocity vector of date
|                    True Equator, True Equinox   ER/TU
|    DPsi96      - NUTATION ANGLE                 rad
|    TrueEps96   - True obliquity of the ecliptic rad
|    Omega       -                                rad
|
|  Locals        :
|    TTT2        - TTT squared
|    TTT3        - TTT cubed
|    TTT4        - TTT to the fourth
|    Eps         - Mean obliquity of the ecliptic rad
|    l           -                                rad
|    ll          -                                rad
|    F           -                                rad
|    D           -                                rad
|    DEps96      - Change in obliquity            rad
|
|  Coupling      :
|    ROT2        - Rotation about the second axis
|    ROT3        - Rotation about the third axis
|    MODFUNC     -
|
|  References    :
|    Vallado       2007, 228
|
  ---------------------------------------------------------------------------- }

PROCEDURE NUT96FK5           ( VAR rMOD,vMOD                         : Vector;
                               Direction                             : Direct;
                               VAR rTOD,vTOD                         : Vector;
                               VAR DPsi96,TrueEps96,Omega            : EXTENDED;
                               TTT                                   : EXTENDED;
                               IAr96                                 : IAr5x263;
                               RAr96                                 : RAr6x263;
                               pIAr96                                : IAr10x112;
                               pRAr96                                : RAr4x112  );
   CONST
     Deg2Rad    : EXTENDED =     0.01745329251994;
   VAR
     Temp, temp1 : Vector;
     i, deg, MIN : INTEGER;
     LonMer,LonVen,LonEar,LonMar,LonJup,LonSat,PrecRate,
{     DEps96,}
{     DPsi96p, DEps96p,}
     Dp, De,
     SEC, Tempval, TTT2, TTT3, TTT4, eps, rr, l, l1, f, d : EXTENDED;

   BEGIN
     { ------- Determine coefficients for IAU 1996 NUTATION Theory ---------- }
     TTT2:= TTT*TTT;
     TTT3:= TTT2*TTT;
     TTT4:= TTT3*TTT;
     Eps := 23.439291 - 0.0130042*TTT - 0.000000164*TTT2 + 0.000000504*TTT3;
     Eps := MODFUNC( Eps,360.0 );
     Eps := Eps * Deg2Rad;

     rr   := 360.0; {deg}
     l    :=  134.96340251 + (1325*rr  + 198.8675605)*TTT + 0.0088553 *TTT2
                           + 0.000014343*TTT3 - 0.00000006797 * TTT4;
     l1   :=  357.52910918 + (  99*rr  + 359.0502911)*TTT - 0.0001537 *TTT2
                           - 0.000000038*TTT3 - 0.00000000319 * TTT4;
     F    :=   93.27209062 + (1342*rr  +  82.0174577)*TTT - 0.0035420 *TTT2
                           + 0.000000288*TTT3 + 0.00000000116 * TTT4;
     D    :=  297.85019547 + (1236*rr  + 307.1114469)*TTT - 0.0017696 *TTT2
                           + 0.000001831*TTT3 - 0.00000000880 * TTT4;
     Omega:=  125.04455501 - (   5*rr  + 134.1361851)*TTT + 0.0020756 *TTT2
                           + 0.000002139*TTT3 - 0.00000001650 * TTT4;
     l    := MODFUNC( l,360.0 )     * Deg2Rad; { rad }
     l1   := MODFUNC( l1,360.0 )    * Deg2Rad;
     F    := MODFUNC( F,360.0 )     * Deg2Rad;
     D    := MODFUNC( D,360.0 )     * Deg2Rad;
     Omega:= MODFUNC( Omega,360.0 ) * Deg2Rad;

     DPsi96:= 0.0;
     DEps96:= 0.0;
     FOR i:= 263 downto 1 DO
       BEGIN
         Tempval:= IAr96[1,i]*l + IAr96[2,i]*l1 + IAr96[3,i]*F + IAr96[4,i]*D + IAr96[5,i]*Omega;
         DPsi96:= DPsi96 + (RAr96[1,i]+RAr96[2,i]*TTT) * SIN( TempVal )
                                         + RAr96[5,i] * COS( TempVal );
         DEps96:= DEps96 + (RAr96[3,i]+RAr96[4,i]*TTT) * COS( TempVal )
                                         + RAr96[6,i] * SIN( TempVal );
       END;

     { --- Determine coefficients for 1996 Planetary NUTATION Theory ----- }
     LonVen  := 181.979800853  +  58517.8156748  *TTT;   { deg }
     LonEar  := 100.466448494  +  35999.3728521  *TTT;
     LonMar  := 355.433274605  +  19140.299314   *TTT;
     LonJup  :=  34.351483900  +   3034.90567464 *TTT;
     LonSat  :=  50.0774713998 +   1222.11379404 *TTT;
     PrecRate:=   1.39697137214*TTT + 0.0003086*TTT2;

     LonVen:= MODFUNC( LonVen,360.0 ) * Deg2Rad;  { rad }
     LonEar:= MODFUNC( LonEar,360.0 ) * Deg2Rad;
     LonMar:= MODFUNC( LonMar,360.0 ) * Deg2Rad;
     LonJup:= MODFUNC( LonJup,360.0 ) * Deg2Rad;
     LonSat:= MODFUNC( LonSat,360.0 ) * Deg2Rad;
     PrecRate:= MODFUNC( PrecRate,360.0 ) * Deg2Rad;

     DPsi96P:= 0.0;
     DEps96P:= 0.0;
     FOR i:= 112 downto 1 DO
       BEGIN
         Tempval:= pIAr96[1,i]*LonVen + pIAr96[2,i]*LonEar + pIAr96[3,i]*LonMar +
                   pIAr96[4,i]*LonJup + pIAr96[5,i]*LonSat + pIAr96[6,i]*PrecRate +
                   pIAr96[7,i]*D + pIAr96[8,i]*F + pIAr96[9,i]*l + pIAr96[10,i]*Omega;
         DPsi96P:= DPsi96P + (pRAr96[1,i]+pRAr96[2,i]*TTT) * SIN( TempVal );
         DEps96P:= DEps96P + (pRAr96[3,i]+pRAr96[4,i]*TTT) * COS( TempVal );
       END;

     { --------------- Find NUTATION Parameters --------------------- }
     { --- Add in Offset and Rate terms since J2000 is referenced --- }
     Dp:= DPsi96;
     De:= DEps96;
     DPsi96 := DPsi96 - 0.0431/3600 - 0.2957/3600*TTT + DPsi96p;
     DEps96 := DEps96 - 0.0051/3600 - 0.0227/3600*TTT + DEps96p;
     DPsi96 := MODFUNC( DPsi96,360.0 ) * Deg2Rad;
     DEps96 := MODFUNC( DEps96,360.0 ) * Deg2Rad;
     TrueEps96 := Eps + DEps96;

     IF Direction = TOO THEN
       BEGIN
         ROT1( rMOD ,    Eps    , Temp  );
         ROT3( Temp , -DPsi96   , Temp1 );
         ROT1( Temp1, -TrueEps96, rTOD  );
         ROT1( vMOD ,    Eps    , Temp  );
         ROT3( Temp , -DPsi96   , Temp1 );
         ROT1( Temp1, -TrueEps96, vTOD  );
       END
       ELSE
       BEGIN
         ROT1( rTOD ,  TrueEps96, Temp  );
         ROT3( Temp ,  DPsi96   , Temp1 );
         ROT1( Temp1,   -Eps    , rMOD  );
         ROT1( vTOD ,  TrueEps96, Temp  );
         ROT3( Temp ,  DPsi96   , Temp1 );
         ROT1( Temp1,   -Eps    , vMOD  );
       END;

     IF Show = 'I' THEN
       BEGIN
         WriteLn( FileOut, '------  96 ~GCRF NUTATION ----------' );
         WriteLn( FileOut, 'Input vars --------- ');
         WriteLn( FileOut, 'Intermediate vars -- ');
         WriteLn( FileOut, 'l       ',l/Deg2Rad:18:12    ,'ø',l:18:12 );
         WriteLn( FileOut, 'l1      ',l1/Deg2Rad:18:12   ,'ø',l1:18:12 );
         WriteLn( FileOut, 'f       ',f/Deg2Rad:18:12    ,'ø',f:18:12 );
         WriteLn( FileOut, 'd       ',d/Deg2Rad:18:12    ,'ø',d:18:12 );
        DMS_RAD(deg,MIN,SEC,FROM, eps );
         WriteLn( FileOut, 'Eps     ',Eps/Deg2Rad:18:12  ,'ø',deg:3,MIN:3,SEC:9:5,Eps:18:12 );
         WriteLn( FileOut, 'DPsi263 ',Dp:18:12,'ø', Dp*Deg2Rad:18:14 );
         WriteLn( FileOut, 'DEps263 ',De:18:12,'ø',De*Deg2Rad:18:14 );
         WriteLn( FileOut, 'DPsi96p ',DPsi96p:18:12,'ø' );
         WriteLn( FileOut, 'DEps96p ',DEps96p:18:12,'ø' );
         WriteLn( FileOut, 'DEps96  ',DEps96/Deg2Rad:18:12       ,'ø',DEps96:18:12 );
         WriteLn( FileOut, 'Output vars -------- ');
         WriteLn( FileOut, 'DPsi96  ',DPsi96/Deg2Rad:18:12       ,'ø',DPsi96:18:12 );
        DMS_RAD(deg,MIN,SEC,FROM, Trueeps96 );
         WriteLn( FileOut, 'TrueEps96',TrueEps96/Deg2Rad:18:12,'ø',deg:3,MIN:3,SEC:9:5 );
         WriteLn( FileOut, 'omega   ',omega/Deg2Rad:18:12,'ø',omega:18:12 );
       END;
   END;  { PROCEDURE NUT96FK5 }
{}
{ ----------------------------------------------------------------------------
|
|                           PROCEDURE SIDEREAL
|
|  This PROCEDURE converts position and velocity vectors between the
|    True equator True equinox of date (ECI) and the True equator True equinox
|    of date (ECEF).  The results take into account the effects of SIDEREAL
|    Time.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    rTOD        - Position vector of date
|                    True Equator, True Equinox   ER
|    vTOD        - Velocity vector of date
|                    True Equator, True Equinox   ER/TU
|    Direction   - Which set of vars to output    FROM  TOO
|    JDUT1       - Julian Date of UT1             days from 4713 BC
|    Omega       -                                rad
|    DPsi        - NUTATION ANGLE                 rad
|    TrueEps     - True obliquity of the ecliptic rad
|
|  Outputs       :
|    rECEF       - Position vector of date
|                    True Equator, True Equinox   ER
|    vECEF       - Velocity vector of date
|                    True Equator, True Equinox   ER/TU
|
|  Locals        :
|    GMST         - Mean Greenwich SIDEREAL Time   0 to 2Pi rad
|    AST         - Apparent ST                    0 to 2Pi rad
|    Hr          - hour                           hr
|    MIN         - Minutes                        MIN
|    SEC         - seconds                        SEC
|    Temp        - Temporary vector
|    TempVal     - Temporary variable
|
|  Coupling      :
|    ROT2        - Rotation about the second axis
|    ROT3        - Rotation about the third axis
|    MAG         - Magnitude of a vector
|
|  References    :
|    Vallado       2007, 228
|
  ---------------------------------------------------------------------------- }

PROCEDURE SIDEREAL           ( VAR rTOD,vTOD                         : Vector;
                               Direction                             : Direct;
                               VAR rECEF,vECEF                       : Vector;
                               JDUT1,Omega,DPsi,MeanEps,XLOD         : EXTENDED );
   CONST
     TwoPi      : EXTENDED =     6.28318530717959;
     OmegaEarth : EXTENDED =     0.05883359980154919;
     Deg2Rad    : EXTENDED =     0.01745329251994;
   VAR
     Temp   : Vector;
     Hr, MIN : INTEGER;
TUT1,TUsec, ThetaSA,
     EqE, GMST, AST, tempval, SEC : EXTENDED;
   BEGIN
     { ------------------------ Find Mean GMST ----------------------- }
     GMST:= GSTIME( JDUT1 );

     { ------------------------ Find Mean AST ----------------------- }
     IF JDUT1 < 2450450.5 THEN  { 1 Jan 1997 }
         EqE:= DPsi*COS(MeanEps) { rad }
       ELSE
         EqE:= DPsi*COS(MeanEps) + 0.00264*Deg2Rad/3600.0*SIN(Omega)
                                 + 0.000063*Deg2Rad/3600.0*SIN(2.0*Omega);
     AST:= GMST + EqE;

     TUT1:= (JDUT1 - 2451545.0 )/ 36525.0;
     ThetaSA:= 7.29211514670698E-05 * (1.0 - XLOD/86400.0); { rad }

     TUSec := SQRT( POWER(6378.1363,3)/398600.4418 );

     omegaearth:= thetasa*TUSec;

     IF Direction = TOO THEN
       BEGIN
         ROT3( rTOD,   AST  , rECEF );
         ROT3( vTOD,   AST  , Temp  );
         vECEF[1]:= Temp[1] + rECEF[2]*OmegaEarth{RatioSidSol};
         vECEF[2]:= Temp[2] - rECEF[1]*OmegaEarth{RatioSidSol};
         vECEF[3]:= Temp[3];
         MAG( vECEF );
       END
       ELSE
       BEGIN
         ROT3( rECEF, -AST  , rTOD );
         Temp   := vECEF;
         Temp[1]:= vECEF[1] - rECEF[2]*OmegaEarth{RatioSidSol};
         Temp[2]:= vECEF[2] + rECEF[1]*OmegaEarth{RatioSidSol};
         MAG( Temp );
         ROT3( Temp,  -AST  , vTOD );
       END;

     IF Show = 'I' THEN
       BEGIN
         WriteLn( FileOut,'------  SIDEREAL TIME ----------' );
         WriteLn( FileOut,'Input vars --------- ');
         WriteLn( FileOut, 'JDUT1   ',JDUT1:18:12 );
         WriteLn( FileOut, 'omega   ',omega/Deg2Rad:18:12,'ø',omega:18:12 );
         WriteLn( FileOut, 'DPsi    ',DPsi/Deg2Rad:18:12       ,'ø',DPsi:18:12 );
         WriteLn( FileOut, 'MeanEps ',MeanEps/Deg2Rad:18:12,'ø' );
         WriteLn( FileOut, 'XLOD    ',XLOD:14:9 );
         WriteLn( FileOut, 'Intermediate vars -- ');
         WriteLn( FileOut, 'ThetaSA ',ThetaSA:20:16,'rad/tu ' );
         WriteLn( FileOut, 'GMST    ',GMST*57.295779513:18:12,GMST:18:12 );
         WriteLn( FileOut, 'AST     ',AST*57.295779513:18:12,AST:18:12 );
         WriteLn( FileOut, 'OmegaEarth',OmegaEarth:18:12,'xx' );
         HMS_RAD(hr,MIN,SEC,FROM, GMST );
         Write( FileOut, 'GMST (hms) ',hr:3,MIN:3,SEC:9:5);
         HMS_RAD(hr,MIN,SEC,FROM, AST );
         WriteLn( FileOut, ' AST (hms) ',hr:3,MIN:3,SEC:9:5);
         TempVal:= AST-GMST;
         HMS_RAD(hr,MIN,SEC,FROM, tempval );
         WriteLn( FileOut,' EQEQ ',SEC:14:8,'sec');
         WriteLn( Fileout   ,'Eqe    ',EqE*180.0/Pi:20:14,' ',EqE );
         WriteLn( FileOut,'Output vars -------- ');
       END;
   END;  { PROCEDURE SIDEREAL Time }
{}
{ ----------------------------------------------------------------------------
|
|                           PROCEDURE POLARM
|
|  This PROCEDURE converts position and velocity vectors  between the
|    True equator True equinox of date (ECEF) and the True equator True equinox
|    of date (ITRF).  The results take into account the effects of Polar
|    Motion.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    rECEF       - Position vector of date
|                    True Equator, True Equinox   ER
|    vECEF       - Velocity vector of date
|                    True Equator, True Equinox   ER/TU
|    TTT         - Julian Centuries of TDB        centuries
|    Direction   - Which set of vars to output    FROM  TOO
|    xp          - Polar motion coefficient       rad
|    yp          - Polar motion coefficient       rad
|
|  Outputs       :
|    rITRF       - Position vector of date
|                    True Equator, True Equinox   ER
|    vITRF       - Velocity vector of date
|                    True Equator, True Equinox   ER/TU
|
|  Locals        :
|    None.
|
|  Coupling      :
|    ROT2        - Rotation about the second axis
|    ROT3        - Rotation about the third axis
|
|  References    :
|    Vallado       2007, 228
|
  ---------------------------------------------------------------------------- }

PROCEDURE POLARM             ( VAR rECEF,vECEF                       : Vector;
                               Direction                             : Direct;
                               VAR rITRF,vITRF                       : Vector;
                               xp,yp                                 : EXTENDED );
   BEGIN
     IF Direction = TOO THEN
       BEGIN
         rITRF[1]:= rECEF[1] + xp*rECEF[3];
         rITRF[2]:= rECEF[2] - yp*rECEF[3];
         rITRF[3]:= rECEF[3] - xp*rECEF[1] + yp*rECEF[2];
         MAG(rITRF);
         vITRF[1]:= vECEF[1] + xp*vECEF[3];
         vITRF[2]:= vECEF[2] - yp*vECEF[3];
         vITRF[3]:= vECEF[3] - xp*vECEF[1] + yp*vECEF[2];
         MAG(vITRF);
       END
       ELSE
       BEGIN
         rECEF[1]:= rITRF[1] - xp*rITRF[3];
         rECEF[2]:= rITRF[2] + yp*rITRF[3];
         rECEF[3]:= rITRF[3] + xp*rITRF[1] - yp*rITRF[2];
         MAG(rECEF);
         vECEF[1]:= vITRF[1] - xp*vITRF[3];
         vECEF[2]:= vITRF[2] + yp*vITRF[3];
         vECEF[3]:= vITRF[3] + xp*vITRF[1] - yp*vITRF[2];
         MAG(vECEF);
       END;

     IF Show = 'I' THEN
       BEGIN
         WriteLn( FileOut,'------  POLAR MOTION ----------' );
         WriteLn( FileOut,'Input vars --------- ');
         WriteLn( FileOut, 'xp ',xp:12:9,' yp ',yp:12:9,' rad' );
         WriteLn( FileOut, 'xp ',xp*3600.0*57.29577951308:12:9,
                           ' yp ',yp*3600.0*57.29577951308:12:9,' "' );
         WriteLn( FileOut,'Intermediate vars -- ');
         WriteLn( FileOut,'Output vars -------- ');
       END;
   END;  { PROCEDURE Polar Motion }
{}
{ ----------------------------------------------------------------------------
|
|                           PROCEDURE TRUEMEAN
|
|  This PROCEDURE converts position and velocity vectors between the
|    NORAD True Equator Mean Equinox of date and the Mean equator Mean equinox
|    of date (ECI).  The results approximate the effects of NUTATION.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units        Ex Value
|    rMOD        - Position vector of date
|                    Mean Equator, Mean Equinox   ER
|    vMOD        - Velocity vector of date
|                    Mean Equator, Mean Equinox   ER/TU
|    TTT         - Julian Centuries of TDB
|    Direction   - Which set of vars to output    FROM  TOO            TOO
|
|  Outputs       :
|    rTm         - Position vector of date
|                    Mean Equator, True Equinox   ER
|    vTm         - Velocity vector of date
|                    Mean Equator, True Equinox   ER/TU
|    DPsiSp  - NUTATION ANGLE                 rad
|    TrueEpsSp   - True obliquity of the ecliptic rad
|
|  Locals        :
|    TTT2        - TTT squared
|    TTT3        - TTT cubed
|    Eps         - Mean obliquity of the ecliptic rad
|    l           -                                rad
|    ll          -                                rad
|    F           -                                rad
|    D           -                                rad
|    Omega       -                                rad
|    DEps    - Change in obliquity            rad
|
|  Coupling      :
|    ROT2        - Rotation about the second axis
|    ROT3        - Rotation about the third axis
|    MODFUNC     -
|
|  References    :
|    Vallado       2007, 236
|
  ---------------------------------------------------------------------------- }
{}
PROCEDURE TrueMean           ( VAR rMOD,vMOD                         : Vector;
                               Direction                             : Direct;
                               VAR rTM,vTM                           : Vector;
                               VAR DPsiTM,TrueEpsTM                  : EXTENDED;
                               TTT                                   : EXTENDED;
                               IAr80                                 : IAr5x106;
                               RAr80                                 : RAr4x106 );
   CONST
     Deg2Rad    : EXTENDED =     0.01745329251994;
   VAR
     ii, i : INTEGER;
     DEpsTM,Tempval, TTT2, TTT3, eps, rr, l, l1, f, d, Omega : EXTENDED;
   BEGIN
     { ---- Determine coefficients for IAU 1980 NUTATION Theory ----- } 
     TTT2:= TTT*TTT;
     TTT3:= TTT2*TTT;
     Eps := 23.439291 - 0.0130042*TTT - 0.000000164*TTT2 + 0.000000504*TTT3;
     Eps := MODFUNC( Eps,360.0 );
     Eps := Eps * Deg2Rad;

     rr   := 360.0; {deg}
     l    :=  134.9629814 + (1325*rr  + 198.8673981)*TTT + 0.0086972 *TTT2 + 0.00001778*TTT3;
     l1   :=  357.5277233 + (  99*rr  + 359.05034  )*TTT - 0.00016028*TTT2 - 0.00000333*TTT3;
     F    :=   93.2719103 + (1342*rr  +  82.0175381)*TTT - 0.0036825 *TTT2 + 0.00000306*TTT3;
     D    :=  297.8503631 + (1236*rr  + 307.111480 )*TTT - 0.00191417*TTT2 + 0.00000528*TTT3;
     Omega:=  125.0445222 - (   5*rr  + 134.1362608)*TTT + 0.0020708 *TTT2 + 0.00000222*TTT3;
     l    := MODFUNC( l,360.0 )     * Deg2Rad;
     l1   := MODFUNC( l1,360.0 )    * Deg2Rad;
     F    := MODFUNC( F,360.0 )     * Deg2Rad;
     D    := MODFUNC( D,360.0 )     * Deg2Rad;
     Omega:= MODFUNC( Omega,360.0 ) * Deg2Rad;

     DPsiTM:= 0.0;
     DEpsTm:= 0.0;

     FOR ii:= 1 to 4 DO
       BEGIN
{         CASE ii OF
           1,2 : i:= ii;
           3   : i:= 9;
           4   : i:= 31;
         END;   Sort the input file!!
}
         i:= ii;

         Tempval:= IAr80[1,i]*l + IAr80[2,i]*l1 + IAr80[3,i]*F + IAr80[4,i]*D + IAr80[5,i]*Omega;
         DPsiTm:= DPsiTM + (RAr80[1,i]+RAr80[2,i]*TTT) * SIN( TempVal );
         DEpsTm:= DEpsTM + (RAr80[3,i]+RAr80[4,i]*TTT) * COS( TempVal );
       END;

     { --------------- Find Approx Nutation Parameters ---------------- }
     DPsiTM := MODFUNC( DPsiTM,360.0 ) * Deg2Rad;
     DEpsTM := MODFUNC( DEpsTM,360.0 ) * Deg2Rad;
     TrueEpsTM := Eps + DEpsTM;

     IF Direction = TOO THEN
       BEGIN
         rtm[1]:= rmod[1] - DPsiTM*SIN(eps)*rmod[3];
         rtm[2]:= rmod[2] - DEpsTM*rmod[3];
         rtm[3]:= rmod[3] + DPsiTM*SIN(eps)*rmod[1] + DEpsTM*rmod[2];
         MAG( rtm );
         vtm[1]:= vmod[1] - DPsiTM*SIN(eps)*vmod[3];
         vtm[2]:= vmod[2] - DEpsTM*vmod[3];
         vtm[3]:= vmod[3] + DPsiTM*SIN(eps)*vmod[1] + DEpsTM*vmod[2];
         MAG( vtm );
       END
       ELSE
       BEGIN
         rmod[1]:= rtm[1] + DPsiTM*SIN(eps)*rtm[3];
         rmod[2]:= rtm[2] + DEpsTM*rtm[3];
         rmod[3]:= rtm[3] - DPsiTM*SIN(eps)*rtm[1] - DEpsTM*rtm[2];
         MAG( rmod );
         vmod[1]:= vtm[1] + DPsiTM*SIN(eps)*vtm[3];
         vmod[2]:= vtm[2] + DEpsTM*vtm[3];
         vmod[3]:= vtm[3] - DPsiTM*SIN(eps)*vtm[1] - DEpsTM*vtm[2];
         MAG( vmod );
       END;

     IF Show = 'I' THEN
       BEGIN
         WriteLn( FileOut, '------  TRUE MEAN ----------' );
         WriteLn( FileOut, 'Input vars --------- ');
         WriteLn( FileOut, 'TTT     ',TTT:18:12 );
         WriteLn( FileOut, 'Intermediate vars -- ');
         WriteLn( FileOut, 'Output vars -------- ');
         WriteLn( FileOut, 'DelPsi  ',DPsiTM/Deg2Rad:18:12,'ø',DPsiTM*3600/Deg2Rad:18:12,'"' );
         WriteLn( FileOut, 'TrueEps ',TrueEpsTM/Deg2Rad:18:12,'ø' );
         WriteLn( FileOut, 'DelEps  ',DEpsTM/Deg2Rad:18:12,'ø',DEpsTM*3600/Deg2Rad:18:12,'"' );
       END;

   END;  { PROCEDURE TrueMean }
{}
{ ----------------------------------------------------------------------------
|
|                           PROCEDURE FK4
|
|  This PROCEDURE converts vectors between the B1950 and J2000 epochs.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    rJ2000      - Initial J2000 Position vector       ER
|    vJ2000      - Initial J2000 Velocity vector      ER/TU
|    Direction   - Which set of vars to output    FROM  TOO
|
|  Outputs       :
|    rECEF       - Position vector Earth Centered Earth Fixed    ER
|    vECEF       - Velocity vector Earth Centered Earth Fixed  ER/TU
|
|  Locals        :
|    r11,r12,r13 - Components of rotation matrix
|    r21,r22,r23 - Components of rotation matrix
|    r31,r32,r33 - Components of rotation matrix
|
|  Coupling      :
|    MAG         - Vector magnitude
|
|  References    :
|    Vallado       2007, 240
|
  ---------------------------------------------------------------------------- }

PROCEDURE FK4                ( VAR rJ2000,vJ2000                     : Vector;
                               Direction                             : Direct;
                               VAR rFK4,vFK4                         : Vector );
   VAR
     r11,r12,r13,r21,r22,r23,r31,r32,r33:EXTENDED;
   BEGIN
     r11:=  0.9999256794956877;
     r12:= -0.0111814832204662;
     r13:= -0.0048590038153592;

     r21:=  0.0111814832391717;
     r22:=  0.9999374848933135;
     r23:= -0.0000271625947142;

     r31:=  0.0048590037723143;
     r32:= -0.0000271702937440;
     r33:=  0.9999881946043742;
     IF Direction = TOO THEN
       BEGIN
         rFK4[1]:= r11*rJ2000[1] + r21*rJ2000[2] + r31*rJ2000[3];
         rFK4[2]:= r12*rJ2000[1] + r22*rJ2000[2] + r32*rJ2000[3];
         rFK4[3]:= r13*rJ2000[1] + r23*rJ2000[2] + r33*rJ2000[3];
         MAG(rFK4);
         vFK4[1]:= r11*vJ2000[1] + r21*vJ2000[2] + r31*vJ2000[3];
         vFK4[2]:= r12*vJ2000[1] + r22*vJ2000[2] + r32*vJ2000[3];
         vFK4[3]:= r13*vJ2000[1] + r23*vJ2000[2] + r33*vJ2000[3];
         MAG(vFK4);
       END
       ELSE
       BEGIN
         rJ2000[1]:= r11*rFK4[1] + r12*rFK4[2] + r13*rFK4[3];
         rJ2000[2]:= r21*rFK4[1] + r22*rFK4[2] + r23*rFK4[3];
         rJ2000[3]:= r31*rFK4[1] + r32*rFK4[2] + r33*rFK4[3];
         MAG(rJ2000);
         vJ2000[1]:= r11*vFK4[1] + r12*vFK4[2] + r13*vFK4[3];
         vJ2000[2]:= r21*vFK4[1] + r22*vFK4[2] + r23*vFK4[3];
         vJ2000[3]:= r31*vFK4[1] + r32*vFK4[2] + r33*vFK4[3];
         MAG(vJ2000);
       END;

   END;  { PROCEDURE FK4 }

BEGIN

END.  { Unit AstReduc }
{}
