(*     ----------------------------------------------------------------      *)

                               UNIT ASTIOD;

(*   this file contains fundamental astrodynamic procedures and functions
*   relating to the initial orbit determination techniques. see ch 7 for
*   a complete discussion of these routines.
*
*                            companion code for
*               fundamentals of astrodynamics and applications
*                                    2004
*                              by david vallado
*
*       (w) 719-573-2600, email dvallado@agi.com
*
*    current :
*              21 jul 05  david vallado
*                           2nd printing baseline
*    changes :
*              14 may 01  david vallado
*                           2nd edition baseline
*              23 nov 87  david vallado
*                           original baseline
*
*      ----------------------------------------------------------------      *)

                                  INTERFACE

(*     ----------------------------------------------------------------      *)

    Uses
       AstMath,
       AstTime,
       Ast2body;

{  --------- only for testgau test -----------
    var  lambda,BigT, Testamt : EXTENDED;
}
{}

PROCEDURE SITE               ( Latgd,Alt,lon                         : EXTENDED;
                               VAR RSijk,VSijk                       : Vector   );

    { ------------------- Angles-only techniques --------------------- }

PROCEDURE ANGLESLAPLACE      ( TDecl1,TDecl2,TDecl3,
                               TRtAsc1,TRtAsc2,TRtAsc3,
                               JD1,JD2,JD3                           : EXTENDED;
                               RS1,RS2,RS3                           : Vector;
                               VAR r2,v2                             : Vector   );

PROCEDURE ANGLESGAUSS        ( TDecl1,TDecl2,TDecl3,
                               TRtAsc1,TRtAsc2,TRtAsc3,
                               JD1,JD2,JD3                           : EXTENDED;
                               RS1,RS2,RS3                           : Vector;
                               VAR r2,v2                             : Vector   );

    { -------------------- Conversion techniques --------------------- }

PROCEDURE RV_RADEC           ( VAR Rijk,Vijk                         : Vector;
                               Direction                             : Direct;
                               VAR rr,RtAsc,Decl,
                                   DRr,DRtAsc,DDecl                  : EXTENDED );

PROCEDURE RV_TRADEC          ( VAR Rijk,Vijk,RSijk                      : Vector;
                               Direction                             : Direct;
                               VAR Rho,TRtAsc,TDecl,
                               DRho,DTRtAsc,DTDecl                   : EXTENDED;
                               latgd,lon,alt,ttt,jdut1,lod,xp,yp,terms,ddpsi,ddeps : Extended ); 

PROCEDURE RV_RAZEL           ( VAR Rijk,Vijk,RSijk                   : Vector;
                               Latgd,LST                             : EXTENDED;
                               Direction                             : Direct;
                               VAR Rho,Az,El,DRho,DAz,DEl            : EXTENDED );

PROCEDURE RV_ELATLON         ( VAR Rijk,Vijk                         : Vector;
                               Direction                             : Direct;
                               VAR rr,EclLat,EclLon,
                                   DRr,DEclLat,DEclLon               : EXTENDED );

PROCEDURE RVSEZ_RAZEL        ( VAR Rhosez,DRhosez                    : Vector;
                               Direction                             : Direct;
                               VAR Rho,Az,El,DRho,DAz,DEl            : EXTENDED );

PROCEDURE RADEC_ELATLON      ( VAR RtAsc,Decl                        : EXTENDED;
                               Direction                             : Direct;
                               VAR EclLat, EclLon                    : EXTENDED );

PROCEDURE RADEC_AZEL         ( VAR RtAsc,Decl,LST,latgd              : EXTENDED;
                               Direction                             : Direct;
                               VAR Az,El                             : EXTENDED );


    { -------------------- Three vector techniques ------------------- }

PROCEDURE GIBBS              ( R1,R2,R3                              : Vector;
                               VAR V2                                : Vector;
                               VAR Theta,Theta1,Copa                 : EXTENDED;
                               VAR Error                             : Str12    );

PROCEDURE HERRGIBBS          ( R1,R2,R3                              : Vector;
                               JD1,JD2,JD3                           : EXTENDED;
                               VAR V2                                : Vector;
                               VAR Theta,Theta1,Copa                 : EXTENDED;
                               VAR Error                             : Str12    );

    { ----------------------- Lambert techniques -------------------- }

PROCEDURE LAMBERTUNIV        ( ro,r                                  : Vector;
                               dm,OverRev                            : CHAR;
                               DtTU                                  : EXTENDED;
                               VAR vo,v                              : Vector;
                               VAR Error                             : Str12    );

PROCEDURE LAMBERTBATTIN      ( ro,r                                  : Vector;
                               dm,OverRev                            : CHAR;
                               DtTU                                  : EXTENDED;
                               VAR vo,v                              : Vector;
                               VAR Error                             : Str12    );

PROCEDURE TARGET             ( RInt,VInt,RTgt,VTgt                   : Vector;
                               Dm,Kind                               : CHAR;
                               DtTU                                  : EXTENDED;
                               VAR V1t,V2t,DV1,DV2                   : Vector;
                               VAR Error                             : Str12    );

{ ------------------- Constants used in this Library ------------------
|
|     Small      : EXTENDED =     0.00000001;        Small value for tolerances
|     HalfPi     : EXTENDED =     1.57079632679490;
|     Pi         : EXTENDED =     3.14159265358979;
|     TwoPi      : EXTENDED =     6.28318530717959;
|     EESqrd     : EXTENDED =     0.006694385000;    Eccentricity of Earth Sqrd
|     OmegaEarth : EXTENDED =     0.05883359221938136;  Earth Rot rad/TU
|     TUDay      : EXTENDED =     0.00933809017716; TU in a solar day
|     TUMin      : EXTENDED =    13.44684985511;
|
 ----------------------------------------------------------------------- }

(*     ----------------------------------------------------------------      *)

                                IMPLEMENTATION

(*     ----------------------------------------------------------------      *)
{}
{ ---------------------------------------------------------------------------
|
|                           PROCEDURE SITE
|
|  This PROCEDURE finds the position and velocity vectors for a SITE.  The
|    answer is returned in the Geocentric Equatorial (ecef) coordinate system.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Latgd       - Geodetic Latitude              -Pi/2 to Pi/2 rad
|    Alt         - Altitude                       km
|    LST         - Local SIDEREAL Time            -2Pi to 2Pi rad
|
|  OutPuts       :
|    RSijk       - IJK SITE position vector       km
|    VSijk       - IJK SITE velocity vector       km/s
|
|  Locals        :
|    SinLat      - Variable containing  SIN(Lat)  rad
|    Temp        - Temporary Real value
|    Rdel        - Rdel component of SITE vector  km
|    Rk          - Rk component of SITE vector    km
|    CEarth      -
|
|  Coupling      :
|    MAG           Magnitude of a vector
|    CROSS         CROSS product of two vectors
|
|  References    :
|    Vallado       2001, 404-407, Alg 47, Ex 7-1
|
 ---------------------------------------------------------------------------- }

PROCEDURE SITE               ( Latgd,Alt,lon                         : EXTENDED;
                               VAR RSijk,VSijk                             : Vector   );
   CONST
     EESqrd     : EXTENDED = 0.00669437999013;
   VAR
     EarthRate          : Vector;
     SinLat, CEarth, Rdel, Rk : EXTENDED;
   BEGIN
     { ---------------------  Initialize values   ------------------- }
     SinLat      := SIN( Latgd );

     { -------  Find Rdel and Rk components of SITE vector  --------- }
     CEarth:= 6378.137 / SQRT( 1.0 - ( EESqrd*SinLat*SinLat ) );
     Rdel  := ( CEarth + Alt )*COS( Latgd );
     Rk    := ( (1.0-EESqrd)*CEarth + Alt )*SinLat;

     { ----------------  Find SITE position vector  ----------------- }
     RSijk[1] := Rdel * COS( lon );
     RSijk[2] := Rdel * SIN( lon );
     RSijk[3] := Rk;
     MAG( RSijk );

     { ----------------  Find SITE velocity vector  ----------------- }
     vsijk[1]:= 0.0;
     vsijk[2]:= 0.0;
     vsijk[3]:= 0.0;

     IF Show='Y' THEN
       BEGIN
         WriteLn( FileOut,'RdelRk ',Rdel:11:7,Rk:11:7,' CEarth ',CEarth:11:7,(1-EESqrd)*CEarth:11:7 );
         WriteLn( FileOut,'RSijk ',RSijk[1]:11:7,RSijk[2]:11:7,RSijk[3]:11:7 );
       END;
   END;  { PROCEDURE SITE }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE ANGLESLAPLACE
|
|  This PROCEDURE solves the problem of orbit determination using three
|    optical sightings and the method of Laplace.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    TRtAsc1      - Right Ascension #1            rad
|    TRtAsc2      - Right Ascension #2            rad
|    TRtAsc3      - Right Ascension #3            rad
|    TDecl1       - Declination #1                rad
|    TDecl2       - Declination #2                rad
|    TDecl3       - Declination #3                rad
|    JD1          - Julian Date of 1st sighting   Days from 4713 BC
|    JD2          - Julian Date of 2nd sighting   Days from 4713 BC
|    JD3          - Julian Date of 3rd sighting   Days from 4713 BC
|    RS1          - IJK SITE position vector #1   ER
|    RS2          - IJK SITE position vector #2   ER
|    RS3          - IJK SITE position vector #3   ER
|
|  OutPuts        :
|    R            - IJK position vector           ER
|    V            - IJK velocity vector           ER / TU
|
|  Locals         :
|    L1           - Line of SIGHT vector for 1st
|    L2           - Line of SIGHT vector for 2nd
|    L3           - Line of SIGHT vector for 3rd
|    LDot         - 1st derivative of L2
|    LDDot        - 2nd derivative of L2
|    RS2Dot       - 1st Derivative of RS2 - vel
|    RS2DDot      - 2nd Derivative of RS2
|    t12t13       - (t1-t2) * (t1-t3)
|    t21t23       - (t2-t1) * (t2-t3)
|    t31t32       - (t3-t1) * (t3-t2)
|    i            - index
|    D            -
|    D1           -
|    D2           -
|    D3           -
|    D4           -
|    OldR         - Previous iteration on r
|    Rho          - Range from SITE to satellite at t2
|    RhoDot       -
|    DMat         -
|    D1Mat        -
|    D2Mat        -
|    D3Mat        -
|    D4Mat        -
|    EarthRate    - Angular rotation of the earth
|    L2DotRS      - Vector L2 Dotted with RSijk
|    Temp         - Temporary vector
|    Temp1        - Temporary vector
|    Small        - Tolerance
|    Roots        -
|
|  Coupling       :
|    MAG          - Magnitude of a vector
|    DETERMINANT  - Evaluate the determinant of a matrix
|    CROSS        - CROSS product of two vectors
|    NORM         - Normlize a matrix
|    ASSIGNVAL    - Assign a value to a matrix
|    GETVAL       - Get a value from a matrix
|    INITMATRIX   - Initialize a matrix and fil with 0.0's
|    DELMATRIX    - Delete a matrix
|    FACTOR       - Find the roots of a polynomial
|
|  References     :
|    Vallado       2001, 413-417
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE ANGLESLAPLACE      ( TDecl1,TDecl2,TDecl3,
                               TRtAsc1,TRtAsc2,TRtAsc3,
                               JD1,JD2,JD3                           : EXTENDED;
                               RS1,RS2,RS3                           : Vector;
                               VAR r2,v2                             : Vector );
   CONST
     OmegaEarth : EXTENDED =     0.05883359980154919;
     TUDay      : EXTENDED =     0.00933809017716;
     {  TUDay:= 58.132440906;}
     Mu         : EXTENDED =     1.0;
     Small      : EXTENDED =     0.0000001;
   VAR
     Poly   : Array16;
     ROOTS  : Array15by2;
     i, j, k : INTEGER;
     DMat, DMat1, DMat2, DMat3, DMat4  : Matrix;
     L1, L2, L3, LDot, LDDot, RS2Dot, RS2DDot, EarthRate, Temp, Temp1 : Vector;
     D, D1, D2, D3, D4, Rho, RhoDot, t1t13, t1t3, t31t3,
    temproot,
     tau1, tau3, BigR2, L2DotRS  : EXTENDED;
    tc,chg:CHAR; ScHi: BYTE;
   BEGIN
     { ----------------------   Initialize   ------------------------ }
     DMat:= NIL;  DMat1:= NIL;  DMat2:= NIL;  DMat3:= NIL;  DMat4:= NIL;
     EarthRate[1]:= 0.0;
     EarthRate[2]:= 0.0;
     EarthRate[3]:= OmegaEarth;
     MAG(EarthRate);

     JD1:= JD1/TUDay;   { days to TU }
     JD2:= JD2/TUDay;
     JD3:= JD3/TUDay;

     { ---- set middle to 0, deltas to other times ---- }
     tau1:= JD1-JD2;
     tau3:= JD3-JD2;

     { --------------- Find Line of SIGHT vectors ------------------- }
     L1[1]:= COS(TDecl1)*COS(TRtAsc1);
     L1[2]:= COS(TDecl1)*SIN(TRtAsc1);
     L1[3]:= SIN(TDecl1);
     L2[1]:= COS(TDecl2)*COS(TRtAsc2);
     L2[2]:= COS(TDecl2)*SIN(TRtAsc2);
     L2[3]:= SIN(TDecl2);
     L3[1]:= COS(TDecl3)*COS(TRtAsc3);
     L3[2]:= COS(TDecl3)*SIN(TRtAsc3);
     L3[3]:= SIN(TDecl3);

     { --------------------------------------------------------------
     |  Using Lagrange Interpolation formula to derive an expression
     |  for L(t), substitute t=t2 and differentiate to obtain the
     |  derivatives of L.
      --------------------------------------------------------------- }
     t1t13:= 1.0 / (tau1*(tau1-tau3));
     t1t3 := 1.0 / (tau1*tau3);
     t31t3:= 1.0 / ((tau3-tau1)*tau3);
     FOR i:= 1 to 3 DO
       BEGIN
         LDot[i]:=      ( -tau3 * t1t13 )*L1[i] +
                 ( (-tau1-tau3) * t1t3  )*L2[i] +
                        ( -tau1 * t31t3 )*L3[i];
         LDDot[i]:= ( 2.0 * t1t13 )*L1[i] +
                    ( 2.0 * t1t3  )*L2[i] +
                    ( 2.0 * t31t3 )*L3[i];
       END;
     MAG( LDot );
     MAG( LDDot );
     NORM( LDot,  LDot );
     NORM( LDDot, LDDot );

     { -------------------- Find 2nd derivative of RSijk --------------- }
     CROSS( RS1,RS2, Temp );
     CROSS( RS2,RS3, Temp1 );

     { needs a different test xxxx!! }
     IF ( ABS(Temp[4]) > Small ) and ( ABS( Temp1[4]) > Small )  THEN
       BEGIN
         { ------- All sightings from one SITE --------- }
         { fix this testhere }
         FOR i:= 1 to 3 DO
           BEGIN { eSc pg 268  doesn't seem to work!!! xx  }
             RS2Dot[i]:=      ( -tau3 * t1t13 )*RS1[i] +
                       ( (-tau1-tau3) * t1t3  )*RS2[i] +
                              ( -tau1 * t31t3 )*RS3[i];
             RS2DDot[i]:= ( 2.0 * t1t13 )*RS1[i] +
                          ( 2.0 * t1t3  )*RS2[i] +
                          ( 2.0 * t31t3 )*RS3[i];
           END; { For }

         CROSS( EarthRate,RS2,     RS2Dot );
         CROSS( EarthRate,RS2Dot,  RS2DDot );
       END
       ELSE
       BEGIN
         { ---- Each sighting from a different SITE ---- }
         FOR i:= 1 to 3 DO
           BEGIN
             RS2Dot[i]:=      ( -tau3 * t1t13 )*RS1[i] +
                       ( (-tau1-tau3) * t1t3  )*RS2[i] +
                              ( -tau1 * t31t3 )*RS3[i];
             RS2DDot[i]:= ( 2.0 * t1t13 )*RS1[i] +
                          ( 2.0 * t1t3  )*RS2[i] +
                          ( 2.0 * t31t3 )*RS3[i];
           END;
       END;

     INITMATRIX( 3,3,1,1, DMat  );
     INITMATRIX( 3,3,1,1, DMat1 );
     INITMATRIX( 3,3,1,1, DMat2 );
     INITMATRIX( 3,3,1,1, DMat3 );
     INITMATRIX( 3,3,1,1, DMat4 );

     FOR i:= 1 to 3 DO
       BEGIN
         ASSIGNVAL( DMat,i,1, 2.0 * L2[i] );
         ASSIGNVAL( DMat,i,2, 2.0 * LDot[i] );
         ASSIGNVAL( DMat,i,3, 2.0 * LDDot[i] );

         { ----  Position determinants ---- }
         ASSIGNVAL( DMat1,i,1, L2[i] );
         ASSIGNVAL( DMat1,i,2, LDot[i] );
         ASSIGNVAL( DMat1,i,3, RS2DDot[i] );
         ASSIGNVAL( DMat2,i,1, L2[i] );
         ASSIGNVAL( DMat2,i,2, LDot[i] );
         ASSIGNVAL( DMat2,i,3, RS2[i] );

         { ----  Velocity determinants ---- }
         ASSIGNVAL( DMat3,i,1, L2[i] );
         ASSIGNVAL( DMat3,i,2, RS2DDot[i] );
         ASSIGNVAL( DMat3,i,3, LDDot[i] );
         ASSIGNVAL( DMat4,i,1, L2[i] );
         ASSIGNVAL( DMat4,i,2, RS2[i] );
         ASSIGNVAL( DMat4,i,3, LDDot[i] );
       END; { for }

     D := DETERMINANT(DMat,3);
     D1:= DETERMINANT(DMat1,3);
     D2:= DETERMINANT(DMat2,3);
     D3:= DETERMINANT(DMat3,3);
     D4:= DETERMINANT(DMat4,3);
(*
|      { --------------  Iterate to find Rho magnitude --------------- }
|     r[4]:= 1.5;  { First Guess }
|     WriteLn( 'Input initial guess for r[4] ' );
|     Readln( r[4] );
|     i:= 1;
|     REPEAT
|         OldR:= r[4];
|         Rho:= -2.0*D1/D - 2.0*D2/(r[4]*r[4]*r[4]*D);
|         r[4]:= SQRT( Rho*Rho + 2.0*Rho*L2DotRS + RS2[4]*RS2[4] );
|         INC(i);
|         r[4]:= (OldR - r[4] ) / 2.0;            { Simple bissection }
|         WriteLn( FileOut,'Rho guesses ',i:2,'Rho ',Rho:14:7,' r[4] ',r[4]:14:7,oldr:14:7 );
|{ seems to converge, but wrong Numbers }
|         INC(i);
|     UNTIL ( ABS( OldR-R[4] ) < Small ) or ( i >= 30 );
*)
     IF ABS(D) > 0.000001 THEN
       BEGIN
         { ---------------- Solve eighth order poly ----------------- }
         L2DotRS:= DOT( L2,RS2 );
         Poly[ 1]:=  1.0; { r2^8th variable!!!!!!!!!!!!!! }
         Poly[ 2]:=  0.0;
         Poly[ 3]:=  (L2DotRS*4.0*D1/D - 4.0*D1*d1/(D*D) - RS2[4]*RS2[4] );
         Poly[ 4]:=  0.0;
         Poly[ 5]:=  0.0;
         Poly[ 6]:=  Mu*(L2DotRS*4.0*D2/D - 8.0*D1*D2/(D*D) );
         Poly[ 7]:=  0.0;
         Poly[ 8]:=  0.0;
         Poly[ 9]:=  -4.0*Mu*D2*D2/(D*D);
         Poly[10]:=  0.0;
         Poly[11]:=  0.0;
         Poly[12]:=  0.0;
         Poly[13]:=  0.0;
         Poly[14]:=  0.0;
         Poly[15]:=  0.0;
         Poly[16]:=  0.0;
         FACTOR( Poly,8,  Roots );

         { ---- Find correct (xx) root ---- }
         BigR2:= 0.0;
         FOR j:= 1 to 8 DO
           BEGIN
             IF ABS( Roots[j,2] ) < Small THEN
               BEGIN
                 WriteLn( 'Root ',j,Roots[j,1],' + ',Roots[j,2],'j' );
     temproot:= roots[j,1]*roots[j,1];
     temproot:= Temproot*TempRoot*TempRoot*TempRoot +
               Poly[3]*TempRoot*TempRoot*TempRoot + Poly[6]*roots[j,1]*Temproot + Poly[9];
                 WriteLn( FileOut,'Root ',j,Roots[j,1],' + ',Roots[j,2],'j  value = ',temproot );
                 IF Roots[j,1] > BigR2 THEN
                     BigR2:= Roots[j,1];
               END; { IF }
           END;   { For j through the roots }
     WriteLn( 'BigR2 ',BigR2:14:7 );   WriteLn( FileOut,'BigR2 ',BigR2:14:7 );
     Write( 'Keep this root ? ' );
     Readln(BigR2 );
{     Finput( BigR2,'R','N',20,12,WhereX,WhereY,Tc,Chg,ScHi );  WriteLn;}

         Rho:= -2.0*D1/D - 2.0*Mu*D2 / (BigR2*BigR2*BigR2*D);

         { ---- Find the middle position vector ---- }
         FOR k:= 1 to 3 DO
             r2[k]:= Rho*L2[k] + RS2[k];
         MAG( r2 );
         { -------- Find RhoDot magnitude -------- }
         RhoDot:= -D3/D - Mu*D4/(r2[4]*r2[4]*r2[4]*D);
     WriteLn( FileOut,'Rho ',Rho:14:7 );
     WriteLn( FileOut,'RhoDot ',RhoDot:14:7 );

         { ----- Find middle velocity vector ----- }
         FOR i:= 1 to 3 DO
             V2[i]:= RhoDot*L2[i] + Rho*LDot[i] + RS2Dot[i];
         MAG( V2 );
      END
      ELSE
        WriteLn( 'Determinant value was zero ',D );

 WriteLn( FileOut,'t123 ',JD1:18:7,JD2:18:7,JD3:18:7,' TU' );
 WriteLn( FileOut,'t123 ',JD1*TUDay:18:7,JD2*TUDay:18:7,JD3*TUDay:18:7,' Days' );
 WriteLn( FileOut,'tau  ',tau1:11:7,tau3:11:7,' TU ' );
 WriteLn( FileOut,'tau  ',tau1*13.446849:11:7,tau3*13.446849:11:7,' MIN ' );
 WriteLn( FileOut,'delta123 ',TDecl1*57.2957:12:6,TDecl2*57.2957:12:6,TDecl3*57.2957:12:6 );
 WriteLn( FileOut,'RtAsc123 ',TRtAsc1*57.2957:12:6,TRtAsc2*57.2957:12:6,TRtAsc3*57.2957:12:6 );
 WriteLn( FileOut,'RtAsc1   ',RS1[1]:12:6,RS1[2]:12:6,RS1[3]:12:6 );
 WriteLn( FileOut,'RtAsc 2  ',RS2[1]:12:6,RS2[2]:12:6,RS2[3]:12:6 );
 WriteLn( FileOut,'RtAsc  3 ',RS3[1]:12:6,RS3[2]:12:6,RS3[3]:12:6 );
 WriteLn( FileOut,'los 1    ',l1[1]:12:6,l1[2]:12:6,l1[3]:12:6 );
 WriteLn( FileOut,'los 2    ',l2[1]:12:6,l2[2]:12:6,l2[3]:12:6 );
 WriteLn( FileOut,'los 3    ',l3[1]:12:6,l3[2]:12:6,l3[3]:12:6 );
 WriteLn( FileOut,'LDot     ',LDot[1]:12:6,LDot[2]:12:6,LDot[3]:12:6 );
 WriteLn( FileOut,'LDDot    ',LDDot[1]:12:6,LDDot[2]:12:6,LDDot[3]:12:6 );
 WriteLn( FileOut,'RS2     ',RS2[1]:12:6,RS2[2]:12:6,RS2[3]:12:6 );
 WriteLn( FileOut,'RS2Dot  ',RS2Dot[1]:12:6,RS2Dot[2]:12:6,RS2Dot[3]:12:6 );
 WriteLn( FileOut,'RS2DDot ',RS2DDot[1]:12:6,RS2DDot[2]:12:6,RS2DDot[3]:12:6 );
 PRINTMAT( DMat,6,' D Matrix ' );   PRINTMAT( DMat1,6,' D1 Matrix ' );
 PRINTMAT( DMat2,6,' D2 Matrix ' ); PRINTMAT( DMat3,6,' D3 Matrix ' );
 PRINTMAT( DMat4,6,' D4 Matrix ' );
 WriteLn(FileOut,'D 01234 = ',d:12:6,d1:12:6,' ',d2:12:6,' ',d3:12:6,' ',d4:12:6 );
     DELMATRIX( DMat );
     DELMATRIX( DMat1 );
     DELMATRIX( DMat2 );
     DELMATRIX( DMat3 );
     DELMATRIX( DMat4 );
   END; { PROCEDURE ANGLESLAPLACE }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE ANGLESGAUSS
|
|  This PROCEDURE solves the problem of orbit determination using three
|    optical sightings.  The solution PROCEDURE uses the Gaussian technique.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    TRtAsc1      - Right Ascension #1            rad
|    TRtAsc2      - Right Ascension #2            rad
|    TRtAsc3      - Right Ascension #3            rad
|    TDecl1       - Declination #1                rad
|    TDecl2       - Declination #2                rad
|    TDecl3       - Declination #3                rad
|    JD1          - Julian Date of 1st sighting   Days from 4713 BC
|    JD2          - Julian Date of 2nd sighting   Days from 4713 BC
|    JD3          - Julian Date of 3rd sighting   Days from 4713 BC
|    RSijk        - IJK SITE position vector      ER
|
|  OutPuts        :
|    R            - IJK position vector at t2     ER
|    V            - IJK velocity vector at t2     ER / TU
|
|  Locals         :
|    L1           - Line of SIGHT vector for 1st
|    L2           - Line of SIGHT vector for 2nd
|    L3           - Line of SIGHT vector for 3rd
|    Tau          - Taylor expansion series about
|                   Tau ( t - to )
|    TauSqr       - Tau squared
|    t21t23       - (t2-t1) * (t2-t3)
|    t31t32       - (t3-t1) * (t3-t2)
|    i            - index
|    D            -
|    Rho          - Range from SITE to sat at t2  ER
|    RhoDot       -
|    DMat         -
|    RS1          - SITE vectors
|    RS2          -
|    RS3          -
|    EarthRate    - Velocity of Earth rotation
|    P            -
|    Q            -
|    OldR         -
|    OldV         -
|    F1           - F coefficient
|    G1           -
|    F3           -
|    G3           -
|    L2DotRS      -
|
|  Coupling       :
|    MAG          - Magnitude of a vector
|    Detrminant   - Evaluate the determinant of a matrix
|    FACTOR       - Find roots of a polynomial
|    MATMULT      - Multiply two matrices together
|    ASSIGNVAL    - Assign a value to a matrix
|    GETVAL       - Get a value from a matrix
|    INITMATRIX   - Initialize a matrix and fil with 0.0's
|    DELMATRIX    - Delete a matrix
|    GIBBS        - GIBBS method of orbit determination
|    HGIBBS       - Herrick GIBBS method of orbit determination
|    ANGLE        - ANGLE between two vectors
|
|  References     :
|    Vallado       2001, 417-421, Alg 49, Ex 7-2 (425-427)
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE ANGLESGAUSS        ( TDecl1,TDecl2,TDecl3,
                               TRtAsc1,TRtAsc2,TRtAsc3,
                               JD1,JD2,JD3                           : EXTENDED;
                               RS1,RS2,RS3                           : Vector;
                               VAR R2,V2                             : Vector   );
   CONST
     {  TUDay:= 58.132440906;}
     TUDay      : EXTENDED =     0.00933809017716;
     Mu         : EXTENDED =     1.0;
     Small      : EXTENDED =     0.0000001;
   VAR
     Error     : Str12;
     ll, i,j   : INTEGER;
     Poly      : Array16;
     ROOTS     : Array15by2;
     r1, r3, L1, L2, L3 : Vector;
     LMatIi, CMat, RhoMat, LMatI, RSMat, LIR  : Matrix;
     rDot, tau1, tau3, u, uDot, p,
     f1, g1, f3, g3, a, ecc, incl, omega, argp, Nu, m, l, ArgPer,
     BigR2, a1, a1u, a3, a3u, d, d1, d2, c1, c3, L2DotRS,
     Rhoold1, Rhoold2, Rhoold3, rad,
     temproot, theta, theta1, copa, TauSqr  : EXTENDED;
     tc, chg : CHAR;  ScHi    : BYTE;
   BEGIN
     { ----------------------   Initialize   ------------------------ }
     CMat:= NIL;  RhoMat:= NIL;  LMatI:= NIL;  RSMat:= NIL;  LIR:= NIL;
     LMatIi := NIL;
     rad    := 57.29577951308;

     JD1:= JD1/TUDay;   { Convert days to TU }
     JD2:= JD2/TUDay;
     JD3:= JD3/TUDay;
     { ---- set middle to 0, deltas to other times ---- }
     tau1:= JD1-JD2;
     tau3:= JD3-JD2;

     { ----------------  Find Line of SIGHT vectors  ---------------- }
     L1[1]:= COS(TDecl1)*COS(TRtAsc1);
     L1[2]:= COS(TDecl1)*SIN(TRtAsc1);
     L1[3]:= SIN(TDecl1);

     L2[1]:= COS(TDecl2)*COS(TRtAsc2);
     L2[2]:= COS(TDecl2)*SIN(TRtAsc2);
     L2[3]:= SIN(TDecl2);

     L3[1]:= COS(TDecl3)*COS(TRtAsc3);
     L3[2]:= COS(TDecl3)*SIN(TRtAsc3);
     L3[3]:= SIN(TDecl3);

     { -------------- Find L matrix and determinant ----------------- }
     { -------- Called LMatI since it is only used for determ ------- }
     INITMATRIX( 3,3,1,1, LMatIi );
     INITMATRIX( 3,3,1,1, LMatI );
     INITMATRIX( 3,3,1,1, RSMat );

     FOR i:= 1 to 3 DO
       BEGIN
         ASSIGNVAL( LMatIi,i,1, L1[i] );
         ASSIGNVAL( LMatIi,i,2, L2[i] );
         ASSIGNVAL( LMatIi,i,3, L3[i] );
         ASSIGNVAL( RSMat,i,1, RS1[i] );
         ASSIGNVAL( RSMat,i,2, RS2[i] );
         ASSIGNVAL( RSMat,i,3, RS3[i] );
       END;

     D:= DETERMINANT(LMatIi,3);
     { ------------------- Now assign the inverse ------------------- }
     ASSIGNVAL( LMatI,1,1,( L2[2]*L3[3]-L2[3]*L3[2])/D );
     ASSIGNVAL( LMatI,2,1,(-L1[2]*L3[3]+L1[3]*L3[2])/D );
     ASSIGNVAL( LMatI,3,1,( L1[2]*L2[3]-L1[3]*L2[2])/D );
     ASSIGNVAL( LMatI,1,2,(-L2[1]*L3[3]+L2[3]*L3[1])/D );
     ASSIGNVAL( LMatI,2,2,( L1[1]*L3[3]-L1[3]*L3[1])/D );
     ASSIGNVAL( LMatI,3,2,(-L1[1]*L2[3]+L1[3]*L2[1])/D );
     ASSIGNVAL( LMatI,1,3,( L2[1]*L3[2]-L2[2]*L3[1])/D );
     ASSIGNVAL( LMatI,2,3,(-L1[1]*L3[2]+L1[2]*L3[1])/D );
     ASSIGNVAL( LMatI,3,3,( L1[1]*L2[2]-L1[2]*L2[1])/D );

     MATMULT( LMatI,RSMat,3,3,3,   LIR );

     { ------------- Find f and g series at 1st and 3rd obs --------- }
     { speed by assuming circ sat vel for uDot here ?? }
     { some similartities in 1/6t3t1 ... }
     { ---- keep separated this time ---- }
     a1 :=  Tau3 / (Tau3 - Tau1);
     a1u:=  (Tau3*((tau3-Tau1)*(Tau3-Tau1) - Tau3*Tau3 )) / (6.0*(Tau3 - Tau1));
     a3 := -Tau1 / (Tau3 - Tau1);
     a3u:= -(Tau1*((tau3-Tau1)*(Tau3-Tau1) - Tau1*Tau1 )) / (6.0*(Tau3 - Tau1));

     { ---- Form initial guess of r2 ---- }
     d1:=  GETVAL(LIR,2,1)*a1 - GETVAL(LIR,2,2) + GETVAL(LIR,2,3)*a3;
     d2:=  GETVAL(LIR,2,1)*a1u + GETVAL(LIR,2,3)*a3u;

     { -------- Solve eighth order poly NOT same as LAPLACE --------- }
     L2DotRS:= DOT( L2,RS2 );
     Poly[ 1]:=  1.0; { r2^8th variable!!!!!!!!!!!!!! }
     Poly[ 2]:=  0.0;
     Poly[ 3]:=  -(D1*D1 + 2.0*D1*L2DotRS + RS2[4]*RS2[4]);
     Poly[ 4]:=  0.0;
     Poly[ 5]:=  0.0;
     Poly[ 6]:=  -2.0*Mu*(L2DotRS*D2 + D1*D2);
     Poly[ 7]:=  0.0;
     Poly[ 8]:=  0.0;
     Poly[ 9]:=  -Mu*Mu*D2*D2;
     Poly[10]:=  0.0;
     Poly[11]:=  0.0;
     Poly[12]:=  0.0;
     Poly[13]:=  0.0;
     Poly[14]:=  0.0;
     Poly[15]:=  0.0;
     Poly[16]:=  0.0;
     FACTOR( Poly,8,  Roots );

     { ------------------- Select the correct root ------------------ }
     BigR2:= 0.0;
     FOR j:= 1 to 8 DO
       BEGIN
         IF ABS( Roots[j,2] ) < Small THEN
           BEGIN
   temproot:= roots[j,1]*roots[j,1];
   temproot:= Temproot*TempRoot*TempRoot*TempRoot +
           Poly[3]*TempRoot*TempRoot*TempRoot + Poly[6]*roots[j,1]*Temproot + Poly[9];
             WriteLn( FileOut,'Root ',j,Roots[j,1],' + ',Roots[j,2],'j  value = ',temproot );
      WriteLn( 'Root ',j,Roots[j,1],' + ',Roots[j,2],'j  value = ',temproot );
             IF Roots[j,1] > BigR2 THEN
                 BigR2:= Roots[j,1];
           END; { IF }
       END;   { For j through the roots }
 Write( 'input r2 ' );
 Readln( BigR2);
{ Finput( BigR2,'R','N',20,12,WhereX,WhereY,Tc,Chg,ScHi );    WriteLn;}
     { ------------- Solve matrix with u2 better known -------------- }
     u:= Mu / ( BigR2*BigR2*BigR2 );

     c1:= a1+a1u*u;
     c3:= a3+a3u*u;
     INITMATRIX( 3,1,1,1,CMat );
     ASSIGNVAL( CMat,1,1, -c1 );
     ASSIGNVAL( CMat,2,1, 1.0);
     ASSIGNVAL( CMat,3,1, -c3 );
     MATMULT( LIR,CMat,3,3,1,   RhoMat );

     Rhoold1:=  GETVAL( RhoMat,1,1 )/c1;
     Rhoold2:= -GETVAL( RhoMat,2,1 );
     Rhoold3:=  GETVAL( RhoMat,3,1 )/c3;


  WriteLn( FileOut,' Now start refining the guess ' );

    { --------- Loop through the refining process ------------  WHILE () DO }
    FOR ll:= 1 to 3 DO
       BEGIN
         WriteLn( FileOut,' Iteration #',ll:2 );
         { ---- Now form the three position vectors ----- }
         FOR i:= 1 to 3 DO
           BEGIN
             R1[i]:=  GETVAL(RhoMat,1,1)*L1[i]/c1 + RS1[i];
             R2[i]:= -GETVAL(RhoMat,2,1)*L2[i]    + RS2[i];
             R3[i]:=  GETVAL(RhoMat,3,1)*L3[i]/c3 + RS3[i];
           END;
         MAG( r1 );
         MAG( r2 );
         MAG( r3 );

         GIBBS(r1,r2,r3,  v2,theta,theta1,copa,error );

         IF (Error <> 'ok') and (copa < 1.0/Rad) THEN
           BEGIN
             { ---- HGibbs to get middle vector ---- }
             JD1:= JD1*TUDay;   { Convert TU to days }
             JD2:= JD2*TUDay;
             JD3:= JD3*TUDay;
             HERRGIBBS(r1,r2,r3,JD1,JD2,JD3,  v2,theta,theta1,copa,error );
             WriteLn( FileOut,'hgibbs ' );
           END;

         RV2COE( r2,v2, p,a,ecc,incl,omega,argp,Nu,m,u,l,ArgPer ) ;

 WriteLn( FileOut,'t123 ',JD1:18:7,JD2:18:7,JD3:18:7,' TU' );
 WriteLn( FileOut,'t123 ',JD1*TUDay:18:7,JD2*TUDay:18:7,JD3*TUDay:18:7,' Days' );
 WriteLn( FileOut,'los 1    ',l1[1]:12:6,l1[2]:12:6,l1[3]:12:6 );
 WriteLn( FileOut,'los 2    ',l2[1]:12:6,l2[2]:12:6,l2[3]:12:6 );
 WriteLn( FileOut,'los 3    ',l3[1]:12:6,l3[2]:12:6,l3[3]:12:6 );
 FILEPRINTMAT( RSMat,' RSMat ', 1.0, 3 ,FileOut );
 MATINVERSE(LMatIi,3,LMatI);
 MATMULT( LMatI,LMatIi,3,3,3,   LIR );
 PRINTMAT( LMatI,3,' LMatI Matrix ' );
 PRINTMAT( LIR,6,' I Matrix ' );
 WriteLn( d:14:7 );
 FILEPRINTMAT( LMatI,' Lmat MatInv ', 1.0, 3 ,FileOut );
 MATMULT( LMatI,LMatIi,3,3,3,   LIR );
 PRINTMAT( LIR,6,' should be I Matrix ' );
 PRINTMAT( LIR,6,' LIR Matrix ' );
 WriteLn( FileOut,'tau  ',tau1:11:7,tau3:11:7,' TU ',' u',u:14:7 );
 WriteLn ( FileOut,'a13, u ',a1:11:7,a3:11:7,a1u:11:7,a3u:11:7 );
 WriteLn( FileOut, 'd1, d2 ',d1:11:7, d2:11:7,' lDotr ',L2DotRS:11:7 );
 WriteLn( FileOut,'coeff ',poly[1]:11:7,poly[3]:11:7,poly[6]:11:7,poly[9]:11:7 );
 FILEPRINTMAT( CMat,' c Matrix ',1.0,3,FileOut );
 FILEPRINTMAT( RhoMat,' Rho Matrix ',1.0,3,FileOut );
 WriteLn( FileOut,'r1 ',r1[1]:11:7,r1[2]:11:7,r1[3]:11:7,r1[4]:14:7  );
 WriteLn( FileOut,'r2 ',r2[1]:11:7,r2[2]:11:7,r2[3]:11:7,r2[4]:14:7  );
 WriteLn( FileOut,'r3 ',r3[1]:11:7,r3[2]:11:7,r3[3]:11:7,r3[4]:14:7  );
 write( FileOut,'hgGau r2',r2[1]:12:6,r2[2]:12:6,r2[3]:12:6,error,copa*rad:11:7 );
 WriteLn( FileOut,v2[1]:12:6,v2[2]:12:6,v2[3]:12:6 );
 WriteLn( FileOut,'p=',p:11:7,' a', a:11:7,' e ',ecc:11:7,' i ',incl*Rad:11:7,
                 ' Omeg ',omega*rad:10:6,' argp ',argp*rad:10:6 );
 WriteLn( 'p',p:10:6,' a', a:10:6,' e',ecc:10:6,' i',incl*Rad:10:6,' W',omega*rad:9:5,' w',argp*rad:9:5 );

     IF ll <= 2 THEN
       BEGIN
         { ---- Now get an improved estimate of the f and g series ---- }
      { or can the analytic functions be found now?? }
         u:= Mu / ( r2[4]*r2[4]*r2[4] );
         rDot:= DOT(r2,v2)/r2[4];
         uDot:= (-3.0*Mu*RDot) / SQR( r2[4]*r2[4] );

         TauSqr:= Tau1*Tau1;
         f1:=  1.0 - 0.5*u*TauSqr - (1.0/6.0)*UDot*TauSqr*Tau1
                   + (1.0/24.0) * u*u*TauSqr*TauSqr
                   + (1.0/30.0)*U*UDot*TauSqr*TauSqr*Tau1;
         g1:= Tau1 - (1.0/6.0)*u*Tau1*TauSqr - (1.0/12.0)*UDot*TauSqr*TauSqr
                   + (1.0/120.0)*u*u*TauSqr*TauSqr*Tau1
                   + (1.0/120.0)*u*UDot*TauSqr*TauSqr*TauSqr;
         TauSqr:= Tau3*Tau3;
         f3:=  1.0 - 0.5*u*TauSqr - (1.0/6.0)*UDot*TauSqr*Tau3
                   + (1.0/24.0) * u*u*TauSqr*TauSqr
                   + (1.0/30.0)*U*UDot*TauSqr*TauSqr*Tau3;
         g3:= Tau3 - (1.0/6.0)*u*Tau3*TauSqr - (1.0/12.0)*UDot*TauSqr*TauSqr
                   + (1.0/120.0)*u*u*TauSqr*TauSqr*Tau3
                   + (1.0/120.0)*u*UDot*TauSqr*TauSqr*TauSqr;
 WriteLn( FileOut,'tau1 ',tau1:11:7,' tau3 ',tau3:11:7,' u ',u:14:7, uDot:14:7 );
 WriteLn( FileOut,'f1 ',f1:11:7,g1:11:7,'f3 ',f3:11:7,g3:11:7 );
  END
  ELSE
  BEGIN
         { --- Now use exact method to find f and g --- }
         ANGLE( R1,R2, Theta );
         ANGLE( R2,R3, Theta1 );

         f1:= 1.0 - ( (R1[4]*(1.0 - COS(Theta)) / p ) );
         g1:= ( R1[4]*R2[4]*SIN(-theta) ) / SQRT( p ); { - ANGLE because backwards!! }
         f3:= 1.0 - ( (R3[4]*(1.0 - COS(Theta1)) / p ) );
         g3:= ( R3[4]*R2[4]*SIN(theta1) ) / SQRT( p );

  WriteLn( FileOut,'f1n',f1:11:7,g1:11:7,'f3 ',f3:11:7,g3:11:7,
                 ' thetas',theta*rad:11:7,theta1*rad:11:7 );
  END;
          c1:=  g3 / (f1*g3 - f3*g1);
         c3:= -g1 / (f1*g3 - f3*g1);
         { ---- Solve for all three ranges via matrix equation ---- }
         ASSIGNVAL( CMat,1,1, -c1 );
         ASSIGNVAL( CMat,2,1, 1.0);
         ASSIGNVAL( CMat,3,1, -c3 );
         MATMULT( LIR,CMat,3,3,1,   RhoMat );
      WriteLn( FileOut,'c1, c3 ',c1:11:7,c3:11:7 );
      FILEPRINTMAT( RhoMat,' Rho Matrix ',1.0,3,FileOut );

         { ---- Check for convergence ---- }

       END;  { While the ranges are still changing }

     { ---- Find all three vectors ri ---- }
     FOR i:= 1 to 3 DO
       BEGIN
         R1[i]:=  GETVAL(RhoMat,1,1)*L1[i]/c1 + RS1[i];
         R2[i]:= -GETVAL(RhoMat,2,1)*L2[i]    + RS2[i];
         R3[i]:=  GETVAL(RhoMat,3,1)*L3[i]/c3 + RS3[i];
       END;
     MAG( r1 );
     MAG( r2 );
     MAG( r3 );
 WriteLn( FileOut,'r1 ',r1[1]:11:7,r1[2]:11:7,r1[3]:11:7,r1[4]:14:7  );
 WriteLn( FileOut,'r2 ',r2[1]:11:7,r2[2]:11:7,r2[3]:11:7,r2[4]:14:7  );
 WriteLn( FileOut,'r3 ',r3[1]:11:7,r3[2]:11:7,r3[3]:11:7,r3[4]:14:7  );
   END; { PROCEDURE ANGLESGAUSS }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE RV_RADEC
|
|  This PROCEDURE converts the right ascension and declination values with
|    position and velocity vectors of a satellite. Uses velocity vector to
|    find the solution of singular cases.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
%  inputs          description                    range / units
%    r           -  position vector               km
%    v           -  velocity vector               km/s
%
%  outputs       :
%    rr          - radius of the satellite        km
%    rtasc       - right ascension                rad
%    decl        - declination                    rad
%    drr         - radius of the satellite rate   km/s
%    drtasc      - right ascension rate           rad/s
%    ddecl       - declination rate               rad/s
%
%  locals        :
%    temp        - temporary position vector
%    temp1       - temporary variable
|
|  Coupling      :
|    MAG         - Magnitude of a vector
|    ATAN2       - ARCTAN FUNCTION that resolves the quadrant ambiguities
|    DOT         - DOT product of two vectors
|    ASIN      - Arc sine FUNCTION
|
|  References    :
|    Vallado       2001, 246-248, Alg 25
|
 ----------------------------------------------------------------------------- }

PROCEDURE RV_RADEC           ( VAR Rijk,Vijk                         : Vector;
                               Direction                             : Direct;
                               VAR rr,RtAsc,Decl,DRr,DRtAsc,DDecl    : EXTENDED );
   CONST
     Small      : EXTENDED = 0.00000001;
   VAR
     Temp, Temp1 : EXTENDED;
   BEGIN
     IF Direction = FROM THEN
       BEGIN
         Rijk[1]:= rr*COS(Decl)*COS(RtAsc);
         Rijk[2]:= rr*COS(Decl)*SIN(RtAsc);
         Rijk[3]:= rr*SIN(Decl);
         MAG(Rijk);
         Vijk[1]:= DRr*COS(Decl)*COS(RtAsc) - rr*SIN(Decl)*COS(RtAsc)*DDecl
                   - rr*COS(Decl)*SIN(RtAsc)*DRtAsc;
         Vijk[2]:= DRr*COS(Decl)*SIN(RtAsc) - rr*SIN(Decl)*SIN(RtAsc)*DDecl
                   + rr*COS(Decl)*COS(RtAsc)*DRtAsc;
         Vijk[3]:= DRr*SIN(Decl) + rr*COS(Decl)*DDecl;
         MAG(Vijk);
       END
       ELSE
       BEGIN
         { -------------- Calculate Angles and Rates ---------------- }
         rr:= Rijk[4];
         Temp:= SQRT( Rijk[1]*Rijk[1] + Rijk[2]*Rijk[2] );
         IF Temp < Small THEN
           BEGIN
             Temp1:= SQRT( Vijk[1]*Vijk[1] + Vijk[2]*Vijk[2] );
             IF ABS(Temp1) > Small THEN
                 RtAsc:= ATAN2( Vijk[2]/Temp1 , Vijk[1]/Temp1 )
               ELSE
                 RtAsc:= 0.0;
           END
           ELSE
             RtAsc:= ATAN2( Rijk[2]/Temp , Rijk[1]/Temp );
         Decl:= ASIN( Rijk[3]/Rijk[4] );

         Temp1:= -Rijk[2]*Rijk[2] - Rijk[1]*Rijk[1]; { different now }
         DRr:= DOT(Rijk,Vijk)/rr;
         IF ABS(Temp1) > Small THEN
             DRtAsc:= ( Vijk[1]*Rijk[2] - Vijk[2]*Rijk[1] ) / Temp1
           ELSE
             DRtAsc:= 0.0;
         IF ABS( Temp ) > Small THEN
             DDecl:= ( Vijk[3] - DRr*SIN( Decl ) ) / Temp
           ELSE
             DDecl:= 0.0;
       END;
   END; { PROCEDURE RV_RADEC }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE RV_TRADEC
|
|  This PROCEDURE converts topocentric right-ascension declination with
|    position and velocity vectors. Uses velocity vector to find the
|    solution of singular cases.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
%  inputs          description                    range / units
%    reci        - eci position vector            km
%    veci        - eci velocity vector            km/s
%    latgd       - geodetic latitude              -pi/2 to pi/2 rad
%    lon         - longitude of site              -2pi to 2pi rad
%    alt         - altitude                       km
%    ttt         - julian centuries of tt         centuries
%    jdut1       - julian date of ut1             days from 4713 bc
%    lod         - excess length of day           sec
%    xp          - polar motion coefficient       arc sec
%    yp          - polar motion coefficient       arc sec
%    terms       - number of terms for ast calculation 0,2
%
%  outputs       :
%    rho         - satellite range from site      km
%    rtasc       - topocentric right ascension    0.0 to 2pi rad
%    decl        - topocentric declination        -pi/2 to pi/2 rad
%    drho        - range rate                     km/s
%    daz         - xxazimuth rate                   rad / s
%    del         - xxelevation rate                 rad / s
%
%  locals        :
%    rhoveci     - eci range vector from site     km
%    drhoveci    - eci velocity vector from site  km / s
%    rhoeci      - eci range vector from site     km
%    drhoeci     - sez velocity vector from site  km
%    wcrossr     - cross product result           km / s
%    earthrate   - eci earth's rotation rate vec  rad / s
%    tempvec     - temporary vector
%    temp        - temporary real*8 value
%    temp1       - temporary real*8 value
%    i           - index
|
|  Coupling      :
|    MAG         - Magnitude of a vector
|    ATAN2       - Arc tangent FUNCTION that resolves the quadrant ambiguities
|    ASIN      - Arc sine FUNCTION
|    LNCOM2      - Linear combination of 2 vectors
|    ADDVEC      - Add two vectors
|    DOT         - DOT product of two vectors
|
|  References    :
|    Vallado       2001, 248-250, Alg 26
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE RV_TRADEC          ( VAR Rijk,Vijk,RSijk                      : Vector;
                               Direction                             : Direct;
                               VAR Rho,TRtAsc,TDecl,
                               DRho,DTRtAsc,DTDecl                   : EXTENDED;
                               latgd,lon,alt,ttt,jdut1,lod,xp,yp,terms,ddpsi,ddeps : Extended ); 
   CONST
     Small      : EXTENDED = 0.00000001;
     OmegaEarth : EXTENDED = 0.05883359221938136;  { Earth Rot rad/TU }
   VAR
     i           : INTEGER;
     EarthRate, RhoV, DRhoV, VSijk  : Vector;
     Latgc, temp, temp1 : EXTENDED;
   BEGIN
     Latgc:= ASIN(rsijk[3]/rsijk[4]);

     SITE ( Latgd,Alt,lon, RSijk,VSijk );

     IF Direction = FROM THEN
       BEGIN
         { --------  Calculate Topocentric Vectors ------------------ }
         RhoV[1]:= Rho*COS(TDecl)*COS(TRtAsc);
         RhoV[2]:= Rho*COS(TDecl)*SIN(TRtAsc);
         RhoV[3]:= Rho*SIN(TDecl);
         MAG(RhoV);

         DRhoV[1]:= DRho*COS(TDecl)*COS(TRtAsc)
                    - Rho*SIN(TDecl)*COS(TRtAsc)*DTDecl
                    - Rho*COS(TDecl)*SIN(TRtAsc)*DTRtAsc;
         DRhoV[2]:= DRho*COS(TDecl)*SIN(TRtAsc)
                    - Rho*SIN(TDecl)*SIN(TRtAsc)*DTDecl
                    + Rho*COS(TDecl)*COS(TRtAsc)*DTRtAsc;
         DRhoV[3]:= DRho*SIN(TDecl) + Rho*COS(TDecl)*DTDecl;
         MAG(DRhoV);

         { ------ Find IJK range vector from SITE to satellite ------ }
         ADDVEC( RhoV,RSijk,  Rijk );
         LNCOM2( 1.0, COS(Latgc), DRhoV, VSijk, Vijk );

         IF Show = 'Y' THEN
           BEGIN
             WriteLn( FileOut,'rtb ',RhoV[1]:18:7,RhoV[2]:18:7,RhoV[3]:18:7, RhoV[4]:18:7,'ER' );
             WriteLn( FileOut,'vtb ',DRhoV[1]:18:7,DRhoV[2]:18:7,DRhoV[3]:18:7, DRhoV[4]:18:7 );
          END;
       END
       ELSE
       BEGIN
         { ------ Find IJK range vector from SITE to satellite ------ }
         LNCOM2( 1.0,-1.0, Rijk,RSijk,  RhoV );
         LNCOM2( 1.0, -COS(Latgc), Vijk, VSijk, DRhoV );

         { -------- Calculate Topocentric ANGLE and Rate Values ----- }
         Rho:= RhoV[4];
         Temp:= SQRT( RhoV[1]*RhoV[1] + RhoV[2]*RhoV[2] );
         IF Temp < Small THEN
           BEGIN
             Temp1:= SQRT( DRhoV[1]*DRhoV[1] + DRhoV[2]*DRhoV[2] );
             TRtAsc:= ATAN2( DRhoV[2]/Temp1 , DRhoV[1]/Temp1 );
           END
           ELSE
             TRtAsc:= ATAN2( RhoV[2]/Temp, RhoV[1]/Temp );

         TDecl:= ASIN( RhoV[3]/RhoV[4] );

         Temp1:= -RhoV[2]*RhoV[2] - RhoV[1]*RhoV[1]; { different now }
         DRho:= DOT(RhoV,DRhoV)/Rho;
         IF ABS(Temp1) > Small THEN
             DTRtAsc:= ( DRhoV[1]*RhoV[2] - DRhoV[2]*RhoV[1] ) / Temp1
           ELSE
             DTRtAsc:= 0.0;
         IF ABS( Temp ) > Small THEN
             DTDecl:= ( DRhoV[3] - DRho*SIN( TDecl ) ) / Temp
           ELSE
             DTDecl:= 0.0;

         IF Show = 'Y' THEN
           BEGIN
             WriteLn( FileOut,'rta ',RhoV[1]:18:7,RhoV[2]:18:7,RhoV[3]:18:7, RhoV[4]:18:7,'ER' );
             WriteLn( FileOut,'vta ',DRhoV[1]:18:7,DRhoV[2]:18:7,DRhoV[3]:18:7, DRhoV[4]:18:7 );
           END;
       END;
  END;  { PROCEDURE RV_TRADEC }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE RV_RAZEL
|
|  This PROCEDURE converts Range, Azimuth, and Elevation and their rates with
|    the Geocentric Equatorial (IJK) Position and Velocity vectors.  Notice the
|    value of small as it can affect rate term calculations. Uses velocity
|    vector to find the solution of singular cases.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Rijk        - IJK position vector            ER
|    Vijk        - IJK velocity vector            ER/TU
|    RSijk       - IJK SITE Position Vector       ER
|    Latgd       - Geodetic Latitude              -Pi/2 to Pi/2 rad
|    LST         - Local SIDEREAL Time            -2Pi to Pi rad
|    Direction   - Which set of vars to output    FROM  TOO
|
|  Outputs       :
|    Rho         - Satellite Range from SITE      ER
|    Az          - Azimuth                        0.0 to 2Pi rad
|    El          - Elevation                      -Pi/2 to Pi/2 rad
|    DRho        - Range Rate                     ER / TU
|    DAz         - Azimuth Rate                   rad / TU
|    DEl         - Elevation rate                 rad / TU
|
|  Locals        :
|    RhoVijk     - IJK Range Vector from SITE     ER
|    DRhoVijk    - IJK Velocity Vector from SITE  ER / TU
|    Rhosez      - SEZ Range vector from SITE     ER
|    DRhosez     - SEZ Velocity vector from SITE  ER
|    WCrossR     - CROSS product result           ER / TU
|    EarthRate   - IJK Earth's rotation rate vec  rad / TU
|    TempVec     - Temporary vector
|    Temp        - Temporary EXTENDED value
|    Temp1       - Temporary EXTENDED value
|    i           - Index
|
|  Coupling      :
|    MAG         - Magnitude of a vector
|    ADDVEC      - Add two vectors
|    CROSS       - CROSS product of two vectors
|    ROT3        - Rotation about the 3rd axis
|    ROT2        - Rotation about the 2nd axis
|    ATAN2       - Arc tangent FUNCTION which also resloves quadrants
|    DOT         - DOT product of two vectors
|    RVSEZ_RAZEL - Find R and V from SITE in Topocentric Horizon (SEZ) system
|    LNCOM2      - Combine two vectors and constants
|    ASIN      - Arc sine FUNCTION
|    SGN         - Returns the sign of a variable
|
|  References    :
|    Vallado       2001, 250-255, Alg 27
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE RV_RAZEL           ( VAR Rijk,Vijk,RSijk                   : Vector;
                               Latgd,LST                             : EXTENDED;
                               Direction                             : Direct;
                               VAR Rho,Az,El,DRho,DAz,DEl            : EXTENDED );
   CONST
     HalfPi     : EXTENDED = 0.5 * pi;  { 1.57079632679490; }
     OmegaEarth : EXTENDED = 0.05883359980154919;
     Small      : EXTENDED = 0.0000001;
   VAR
     Temp, Temp1  : EXTENDED;
     Rhoijk, DRhoijk, Rhosez, DRhosez, WCrossR, EarthRate, TempVec : Vector;
   BEGIN
     { --------------------  Initialize values   -------------------- }
     EarthRate[1]:= 0.0;
     EarthRate[2]:= 0.0;
     EarthRate[3]:= OmegaEarth;

     IF Direction = FROM THEN
       BEGIN
         { ---------  Find SEZ range and velocity vectors ----------- }
         RVSEZ_RAZEL( Rhosez,DRhosez, FROM, Rho,Az,El,DRho,DAz,DEl );

         { ----------  Perform SEZ to IJK transformation ------------ }
         ROT2( Rhosez ,Latgd-HalfPi, TempVec );
         ROT3( TempVec,   -LST   , Rhoijk );
         ROT2( DRhosez,Latgd-HalfPi, TempVec );
         ROT3( TempVec,   -LST   , DRhoijk );

         { ---------  Find IJK range and velocity vectors ----------- }
         ADDVEC( Rhoijk,RSijk,Rijk );
         CROSS ( EarthRate,RSijk ,WCrossR );
         ADDVEC( DRhoijk,WCrossR,Vijk );

         IF Show = 'Y' THEN
           BEGIN
             WriteLn( FileOut,'rseb',Rhosez[1]:18:7,Rhosez[2]:18:7,Rhosez[3]:18:7, Rhosez[4]:18:7,'ER' );
             WriteLn( FileOut,'vseb',dRhosez[1]:18:7,dRhosez[2]:18:7,dRhosez[3]:18:7, dRhosez[4]:18:7 );
             WriteLn( FileOut,'RhoV ijk ',Rhoijk[1]:11:7,Rhoijk[2]:11:7,Rhoijk[3]:11:7 );
             WriteLn( FileOut,'DRhovijk ',dRhoijk[1]:11:7,dRhoijk[2]:11:7,dRhoijk[3]:11:7 );
             WriteLn( FileOut,' Mat r1 ',SIN(Latgd)*COS(LST):11:7,-SIN(LST):11:7,COS(Latgd)*COS(LST):11:7 );
             WriteLn( FileOut,' Mat r2 ',SIN(Latgd)*SIN(LST):11:7, COS(LST):11:7,COS(Latgd)*SIN(LST):11:7 );
             WriteLn( FileOut,' Mat r3 ',    -COS(Latgd):11:7,     '0.00':11,    SIN(Latgd):11:7 );
           END;
       END
       ELSE
       BEGIN
         { ------- Find IJK range vector from SITE to satellite ----- }
         CROSS( EarthRate,Rijk,WCrossR );
         LNCOM2( 1.0,-1.0,Rijk,RSijk,    Rhoijk );
         LNCOM2( 1.0,-1.0,Vijk,WCrossR,  DRhoijk );
         Rho:= Rhoijk[4];

         { ------------ Convert to SEZ for calculations ------------- }
         ROT3( Rhoijk ,    LST   ,  TempVec );
         ROT2( TempVec,HalfPi-Latgd,   Rhosez   );
         ROT3( DRhoijk,    LST   ,  TempVec );
         ROT2( TempVec,HalfPi-Latgd,  DRhosez   );

         { ------------ Calculate Azimuth and Elevation ------------- }
         Temp:= SQRT( Rhosez[1]*Rhosez[1] + Rhosez[2]*Rhosez[2] );
         IF ABS( Rhosez[2] ) < Small THEN
             IF Temp < Small THEN
               BEGIN
                 Temp1:= SQRT( DRhosez[1]*DRhosez[1] + DRhosez[2]*DRhosez[2] );
                 Az   := ATAN2( DRhosez[2]/Temp1 , -DRhosez[1]/Temp1 );
               END
               ELSE
                 IF Rhosez[1] > 0.0 THEN
                     Az:= Pi
                   ELSE
                     Az:= 0.0
           ELSE
             Az:= ATAN2( Rhosez[2]/Temp , -Rhosez[1]/Temp );

         IF ( Temp < Small ) THEN   { directly over the north pole }
             El:= SGN(Rhosez[3])*HalfPi { +- 90 }
           ELSE
             El:= ASIN( Rhosez[3] / Rhosez[4] );

         { ----- Calculate Range, Azimuth and Elevation rates ------- }
         DRho:= DOT(Rhosez,DRhosez)/Rho;
         IF ABS( Temp*Temp ) > Small THEN
             DAz:= ( DRhosez[1]*Rhosez[2] - DRhosez[2]*Rhosez[1] ) /
                   ( Temp*Temp )
           ELSE
             DAz:= 0.0;

         IF ABS( Temp ) > 0.00000001 THEN
             DEl:= ( DRhosez[3] - DRho*SIN( El ) ) / Temp
           ELSE
             DEl:= 0.0;

         IF Show = 'Y' THEN
           BEGIN
             WriteLn( FileOut,'rsez',Rhosez[1]:18:7,Rhosez[2]:18:7,Rhosez[3]:18:7, Rhosez[4]:18:7,'ER' );
             WriteLn( FileOut,'vsez',dRhosez[1]:18:7,dRhosez[2]:18:7,dRhosez[3]:18:7, dRhosez[4]:18:7 );
           END;
       END; { IF }
  END; { PROCEDURE RV_RAZEL }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE RV_ELATLON
|
|  This PROCEDURE converts ecliptic latitude and longitude with position and
|    velocity vectors. Uses velocity vector to find the solution of singular
|    cases.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Rijk        - IJK position vector            ER
|    Vijk        - IJK velocity vector            ER/TU
|    Direction   - Which set of vars to output    FROM  TOO
|
|  OutPuts       :
|    Rr          - Radius of the sat              ER
|    EclLat      - Ecliptic Latitude              -Pi/2 to Pi/2 rad
|    EclLon      - Ecliptic Longitude             -Pi/2 to Pi/2 rad
|    DRr         - Radius of the sat rate         ER/TU
|    DEclLat     - Ecliptic Latitude rate         -Pi/2 to Pi/2 rad
|    EEclLon     - Ecliptic Longitude rate        -Pi/2 to Pi/2 rad
|
|  Locals        :
|    Obliquity   - Obliquity of the ecliptic      rad
|    Temp        -
|    Temp1       -
|    Re          - Position vec in eclitpic frame
|    Ve          - Velocity vec in ecliptic frame
|
|  Coupling      :
|    MAG         - Magnitude of a vector
|    ROT1        - Rotation about 1st axis
|    DOT         - DOT product
|    ASIN      - Arc sine FUNCTION
|    ATAN2       - Arc tangent FUNCTION that resolves quadrant ambiguites
|
|  References    :
|    Vallado       2001, 257-259, Eq 4-15
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE RV_ELATLON         ( VAR Rijk,Vijk                         : Vector;
                               Direction                             : Direct;
                               VAR rr,EclLat,EclLon,
                               DRr,DEclLat,DEclLon                   : EXTENDED );
   CONST
     Small      : EXTENDED = 0.00000001;
   VAR
     Re, Ve : Vector;
     Obliquity, Temp, Temp1 : EXTENDED;
   BEGIN
     Obliquity:= 0.40909280; {23.439291/rad;}
     IF Direction = FROM THEN
       BEGIN
         Re[1]:= rr*COS(EclLat)*COS(EclLon);
         Re[2]:= rr*COS(EclLat)*SIN(EclLon);
         Re[3]:= rr*SIN(EclLat);
         MAG(Re);
         Ve[1]:= DRr*COS(EclLat)*COS(EclLon)
                 - rr*SIN(EclLat)*COS(EclLon)*DEclLat
                 - rr*COS(EclLat)*SIN(EclLon)*DEclLon;
         Ve[2]:= DRr*COS(EclLat)*SIN(EclLon)
                 - rr*SIN(EclLat)*SIN(EclLon)*DEclLat
                 + rr*COS(EclLat)*COS(EclLon)*DEclLon;
         Ve[3]:= DRr*SIN(EclLat) + rr*COS(EclLat)*DEclLat;
         MAG(Ve);

         ROT1( Re, -Obliquity, Rijk );
         ROT1( Ve, -Obliquity, Vijk );
       END
       ELSE
       BEGIN
         ROT1( Rijk, Obliquity, Re );
         ROT1( Vijk, Obliquity, Ve );

         { -------------- Calculate Angles and Rates ---------------- }
         rr:= Re[4];
         Temp:= SQRT( Re[1]*Re[1] + Re[2]*Re[2] );
         IF Temp < Small THEN
           BEGIN
             Temp1:= SQRT( Ve[1]*Ve[1] + Ve[2]*Ve[2] );
             IF ABS(Temp1) > Small THEN
                 EclLon:= ATAN2( Ve[2]/Temp1 , Ve[1]/Temp1 )
               ELSE
                 EclLon:= 0.0;
           END
           ELSE
             EclLon:= ATAN2( Re[2]/Temp , Re[1]/Temp );
         EclLat:= ASIN( Re[3]/Re[4] );

         Temp1:= -Re[2]*Re[2] - Re[1]*Re[1]; { different now }
         DRr:= DOT(re,Ve)/rr;
         IF ABS( Temp1 ) > Small THEN
             DEclLon:= ( Ve[1]*Re[2] - Ve[2]*Re[1] ) / Temp1
           ELSE
             DEclLon:= 0.0;
         IF ABS( Temp ) > Small THEN
             DEclLat:= ( Ve[3] - DRr*SIN( EclLat ) ) / Temp
           ELSE
             DEclLat:= 0.0;
       END; { IF }

   END; { PROCEDURE RV_ELATLON }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE RVSEZ_RAZEL
|
|  This PROCEDURE converts range, azimuth, and elevation values with slant
|    range and velocity vectors for a satellite from a radar SITE in the
|    Topocentric Horizon (SEZ) system.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    RhoVec      - SEZ Satellite range vector     ER
|    DRhoVec     - SEZ Satellite velocity vector  ER / TU
|    Direction   - Which set of vars to output    FROM  TOO
|
|  OutPuts       :
|    Rho         - Satellite range from SITE      ER
|    Az          - Azimuth                        0.0 to 2Pi rad
|    El          - Elevation                      -Pi/2 to Pi/2 rad
|    DRho        - Range Rate                     ER / TU
|    DAz         - Azimuth Rate                   rad / TU
|    DEl         - Elevation rate                 rad / TU
|
|  Locals        :
|    SinEl       - Variable for SIN( El )
|    CosEl       - Variable for COS( El )
|    SinAz       - Variable for SIN( Az )
|    CosAz       - Variable for COS( Az )
|    Temp        -
|    Temp1       -
|
|  Coupling      :
|    MAG         - Magnitude of a vector
|    SGN         - Returns the sign of a variable
|    DOT         - DOT product
|    ASIN      - Arc sine FUNCTION
|    ATAN2       - Arc tangent FUNCTION that resolves quadrant ambiguites
|
|  References    :
|    Vallado       2001, 250-251, Eq 4-4, Eq 4-5
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE RVSEZ_RAZEL        ( VAR Rhosez,DRhosez                    : Vector;
                               Direction                             : Direct;
                               VAR Rho,Az,El,DRho,DAz,DEl            : EXTENDED );
   CONST
     Small      : EXTENDED =    0.00000001;
     HalfPi     : EXTENDED =    1.57079632679490;
   VAR
     Temp1, Temp, SinEl, CosEl, SinAz, CosAz : EXTENDED;
   BEGIN
     IF Direction = FROM THEN
       BEGIN
         { -------------------- Initialize values ------------------- }
         SinEl:= SIN(El);
         CosEl:= COS(El);
         SinAz:= SIN(Az);
         CosAz:= COS(Az);

         { ----------------- Form SEZ range vector ------------------ }
         Rhosez[1] := -Rho*CosEl*CosAz;
         Rhosez[2] :=  Rho*CosEl*SinAz;
         Rhosez[3] :=  Rho*SinEl;
         MAG( Rhosez );

         { --------------- Form SEZ velocity vector ----------------- }
         DRhosez[1] := -DRho*CosEl*CosAz + Rhosez[3]*DEl*CosAz + Rhosez[2]*DAz;
         DRhosez[2] :=  DRho*CosEl*SinAz - Rhosez[3]*DEl*SinAz - Rhosez[1]*DAz;
         DRhosez[3] :=  DRho*SinEl       + Rho*DEl*CosEl;
         MAG( DRhosez );
       END
       ELSE
       BEGIN
         { ------------ Calculate Azimuth and Elevation ------------- }
         Temp:= SQRT( Rhosez[1]*Rhosez[1] + Rhosez[2]*Rhosez[2] );
         IF ABS( Rhosez[2] ) < Small THEN
             IF Temp < Small THEN
               BEGIN
                 Temp1:= SQRT( DRhosez[1]*DRhosez[1] + DRhosez[2]*DRhosez[2] );
                 Az   :=  ATAN2( DRhosez[2]/Temp1 , -DRhosez[1]/Temp1 );
               END
               ELSE
                 IF Rhosez[1] > 0.0 THEN
                     Az:= Pi
                   ELSE
                     Az:= 0.0
           ELSE
             Az:= ATAN2( Rhosez[2]/Temp , -Rhosez[1]/Temp );

         IF ( Temp < Small ) THEN   { directly over the north pole }
             El:= SGN(Rhosez[3])*HalfPi { +- 90 }
           ELSE
             El:= ASIN( Rhosez[3] / Rhosez[4] );

         { ------  Calculate Range, Azimuth and Elevation rates ----- }
         DRho:= DOT(Rhosez,DRhosez)/Rho;
         IF ABS( Temp*Temp ) > Small THEN
             DAz:= ( DRhosez[1]*Rhosez[2] - DRhosez[2]*Rhosez[1] ) /
                   ( Temp*Temp )
           ELSE
             DAz:= 0.0;

         IF ABS( Temp ) > Small THEN
             DEl:= ( DRhosez[3] - DRho*SIN( El ) ) / Temp
           ELSE
             DEl:= 0.0;
         IF Show = 'Y' THEN
           BEGIN
             WriteLn( FileOut,'rsez',Rhosez[1]:18:7,Rhosez[2]:18:7,Rhosez[3]:18:7, Rhosez[4]:18:7,'ER' );
             WriteLn( FileOut,'vsez',dRhosez[1]:18:7,dRhosez[2]:18:7,dRhosez[3]:18:7, dRhosez[4]:18:7 );
           END;
       END; { IF }
   END;  { PROCEDURE RVSEZ_RAZEL }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE RADEC_ELATLON
|
|  This PROCEDURE converts right-ascension declination values with ecliptic
|    latitude and longitude values.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    RtAsc       - Right Ascension                rad
|    Decl        - Declination                    rad
|    Direction   - Which set of vars to output    FROM  TOO
|
|  OutPuts       :
|    EclLat      - Ecliptic Latitude              -Pi/2 to Pi/2 rad
|    EclLon      - Ecliptic Longitude             -Pi/2 to Pi/2 rad
|
|  Locals        :
|    Obliquity   - Obliquity of the ecliptic      rad
|    Sinv        -
|    Cosv        -
|
|  Coupling      :
|    ASIN      - Arc sine FUNCTION
|    ATAN2       - Arc tangent FUNCTION that resolves quadrant ambiguites
|
|  References    :
|    Vallado       2001, 259, Eq 4-19, Eq 4-20
|
 ----------------------------------------------------------------------------- }

PROCEDURE RADEC_ELATLON      ( VAR RtAsc,Decl                        : EXTENDED;
                               Direction                             : Direct;
                               VAR EclLat, EclLon                    : EXTENDED );
   VAR
     Sinv, Cosv, Obliquity : EXTENDED;
   BEGIN
     Obliquity:= 0.40909280; {23.439291/rad;}
     IF Direction = FROM THEN
       BEGIN
         Decl := ASIN( SIN(EclLat)*COS(Obliquity)
                         + COS(EclLat)*SIN(Obliquity)*SIN(EclLon) );
         Sinv := ( -SIN(EclLat)*SIN(Obliquity)
                   + COS(EclLat)*COS(Obliquity)*SIN(EclLon) ) / COS(Decl);
         Cosv := COS(EclLat)*COS(EclLon) / COS(Decl);
         RtAsc:= ATAN2( Sinv,Cosv );
       END
       ELSE
       BEGIN
         EclLat:= ASIN( -COS(Decl)*SIN(RtAsc)*SIN(Obliquity)
                          + SIN(Decl)*COS(Obliquity) );
         Sinv  := ( COS(Decl)*SIN(RtAsc)*COS(Obliquity)
                    + SIN(Decl)*SIN(Obliquity) ) / COS(EclLat);
         Cosv  := COS(Decl)*COS(RtAsc) / COS(EclLat);
         EclLon:= ATAN2( Sinv,Cosv );
       END;
   END;  { PROCEDURE RADEC_ELATLON }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE RADEC_AZEL
|
| This PROCEDURE converts right ascension declination values with
|   azimuth, and elevation.  Notice the range is not defined because
|   Right ascension declination only allows a unit vector to be formed.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    RtAsc       - Right Ascension                0.0 to 2Pi rad
|    Decl        - Declination                    -Pi/2 to Pi/2 rad
|    LST         - Local SIDEREAL Time            -2Pi to 2Pi rad
|    Latgd       - Geodetic Latitude              -Pi/2 to Pi/2 rad
|    Direction   - Which set of vars to output    FROM  TOO
|
|  Outputs       :
|    Az          - Azimuth                        0.0 to 2Pi rad
|    El          - Elevation                      -Pi/2 to Pi/2 rad
|
|  Locals        :
|    LHA         - Local Hour ANGLE               -2Pi to 2Pi rad
|    Sinv        - Sine value
|    Cosv        - Cosine value
|
|  Coupling      :
|    ASIN      - Arc sine FUNCTION
|    ATAN2       - Arc Tangent FUNCTION that resolves quadrant ambiguites
|
|  References    :
|    Vallado       2001, 255-257, Alg 28
|
 ----------------------------------------------------------------------------- }

PROCEDURE RADEC_AZEL         ( VAR RtAsc,Decl,LST,Latgd              : EXTENDED;
                               Direction                             : Direct;
                               VAR Az,El                             : EXTENDED );
   VAR
     Sinv, Cosv, LHA :  EXTENDED;
   BEGIN
     IF Direction = FROM THEN
       BEGIN
         Decl := ASIN( SIN(El)*SIN(Latgd) + COS(el)*COS(Latgd)*COS(Az) );

         Sinv := -(SIN(az)*COS(el)*COS(Latgd)) / (COS(Latgd)*COS(Decl));
         Cosv := (SIN(el) - SIN(Latgd)*SIN(decl)) / (COS(Latgd)*COS(Decl));
         LHA  := ATAN2( Sinv,Cosv );
         RtAsc:= LST - LHA;
       END
       ELSE
       BEGIN
         LHA := LST - RtAsc;

         El  := ASIN( SIN(Decl)*SIN(Latgd) + COS(Decl)*COS(Latgd)*COS(LHA) );

         Sinv:= -SIN(LHA)*COS(Decl)*COS(Latgd)/(COS(el)*COS(Latgd));
         Cosv:= ( SIN(Decl)-SIN(el)*SIN(Latgd) )/(COS(el)*COS(Latgd));
         Az  := ATAN2( Sinv,Cosv );
       END;

     IF Show = 'Y' THEN
         WriteLn( FileOut,LHA*180/pi );

   END;  { PROCEDURE RADEC_AZEL }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE GIBBS
|
|  This PROCEDURE performs the GIBBS method of orbit determination.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    R1          - IJK Position vector #1         ER
|    R2          - IJK Position vector #2         ER
|    R3          - IJK Position vector #3         ER
|
|  OutPuts       :
|    V2          - IJK Velocity Vector for R2     ER / TU
|    Theta       - ANGLE between vectors          rad
|    Error       - Flag indicating success        'ok',...
|
|  Locals        :
|    tover2      -
|    l           -
|    Small       - Tolerance for roundoff errors
|    r1mr2       - Magnitude of r1 - r2
|    r3mr1       - Magnitude of r3 - r1
|    r2mr3       - Magnitude of r2 - r3
|    p           - P Vector     r2 x r3
|    q           - Q Vector     r3 x r1
|    w           - W Vector     r1 x r2
|    d           - D Vector     p + q + w
|    n           - N Vector (r1)p + (r2)q + (r3)w
|    s           - S Vector
|                    (r2-r3)r1+(r3-r1)r2+(r1-r2)r3
|    b           - B Vector     d x r2
|    Theta1      - Temp ANGLE between the vectors rad
|    Pn          - P Unit Vector
|    R1N         - R1 Unit Vector
|    dn          - D Unit Vector
|    Nn          - N Unit Vector
|    i           - index
|
|  Coupling      :
|    MAG         - Magnitude of a vector
|    CROSS       - CROSS product of two vectors
|    DOT         - DOT product of two vectors
|    ADD3VEC     - Add three vectors
|    LNCOM2      - Multiply two vectors by two constants
|    LNCOM3      - Add three vectors each multiplied by a constant
|    NORM        - Creates a Unit Vector
|    ANGLE       - ANGLE between two vectors
|
|  References    :
|    Vallado       2001, 432-445, Alg 52, Ex 7-5
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE GIBBS              ( R1,R2,R3                              : Vector;
                               VAR V2                                : Vector;
                               VAR Theta,Theta1,Copa                 : EXTENDED;
                               VAR Error                             : Str12  );
   VAR
     i                                     : INTEGER;
     tover2, l, Small, r1mr2, r3mr1, r2mr3 : EXTENDED;
     p, q, w, d, n, s, b, Pn, R1N, Dn, Nn  : Vector;
   BEGIN
     { --------------------  Initialize values   -------------------- }
     Small:= 0.000001;  Theta:= 0.0;   Error := 'ok';  Theta1:= 0.0;
     MAG( R1 );         MAG( R2 );     MAG( R3 );      Copa:= 0.0;
     FOR i:= 1 to 4 DO
         V2[i]:= 0.0;

     { ----------------------------------------------------------------
     |  Determine IF the vectors are coplanar.
       -------------------------------------------------------------- }
     CROSS( R2,R3,P );
     CROSS( R3,R1,Q );
     CROSS( R1,R2,W );
     NORM( P,Pn );
     NORM( R1,R1N );
     Copa:=  ASIN( DOT( Pn,R1n ) );

     IF ABS( DOT(R1N,Pn) ) > 0.017452406 THEN
         Error:= 'not coplanar';

     { ---------------- or don't continue processing ---------------- }
     ADD3VEC( P,Q,W,D );
     LNCOM3( R1[4],R2[4],R3[4],P,Q,W,N );
     NORM( N,Nn );
     NORM( D,DN );

     { ----------------------------------------------------------------
     |  Determine IF the orbit is possible.  Both D and N must be in
     |    the same direction, and non-zero.
       -------------------------------------------------------------- }
     IF ( ABS(d[4])<Small ) or ( ABS(n[4])<Small ) or
        ( DOT(Nn,dn) < Small ) THEN
         Error:= 'impossible'
       ELSE
         BEGIN
           ANGLE( R1,R2, Theta );
           ANGLE( R2,R3, Theta1 );

           { ------------ Perform GIBBS method to find V2 ----------- }
           R1mr2:= R1[4]-R2[4];
           R3mr1:= R3[4]-R1[4];
           R2mr3:= R2[4]-R3[4];
           LNCOM3(R1mr2,R3mr1,R2mr3,R3,R2,R1,S);
           CROSS( d,r2,b );
           L    := 3.986004418e5 / SQRT(d[4]*n[4]);
           Tover2:= L / R2[4];
           LNCOM2(Tover2,L,B,S,V2);
         END;

     IF ( Show IN ['Y','S']) and ( Error = 'ok' ) THEN
       BEGIN
         WriteLn( FileOut,'P vector = ':16,P[1]:9:3,P[2]:9:3,P[3]:9:3 );
         WriteLn( FileOut,'Q vector = ':16,Q[1]:9:3,Q[2]:9:3,Q[3]:9:3 );
         WriteLn( FileOut,'W vector = ':16,W[1]:9:3,W[2]:9:3,W[3]:9:3 );
         WriteLn( FileOut,'N vector = ':16,N[1]:9:3,N[2]:9:3,N[3]:9:3 );
         WriteLn( FileOut,'D vector = ':16,D[1]:9:3,D[2]:9:3,D[3]:9:3 );
         WriteLn( FileOut,'S vector = ':16,S[1]:9:3,S[2]:9:3,S[3]:9:3 );
         WriteLn( FileOut,'B vector = ':16,B[1]:9:3,B[2]:9:3,B[3]:9:3,' L',l:11:7 );
       END;

   END;  { PROCEDURE GIBBS }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE HERRGIBBS
|
|  This PROCEDURE implements the Herrick-GIBBS approximation for orbit
|    determination, and finds the middle velocity vector for the 3 given
|    position vectors.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    R1          - IJK Position vector #1         ER
|    R2          - IJK Position vector #2         ER
|    R3          - IJK Position vector #3         ER
|    JD1         - Julian Date of 1st sighting    days from 4713 BC
|    JD2         - Julian Date of 2nd sighting    days from 4713 BC
|    JD3         - Julian Date of 3rd sighting    days from 4713 BC
|
|  OutPuts       :
|    V2          - IJK Velocity Vector for R2     ER / TU
|    Theta       - ANGLE between vectors          rad
|    Error       - Flag indicating success        'ok',...
|
|  Locals        :
|    Dt21        - time delta between r1 and r2   TU
|    Dt31        - time delta between r3 and r1   TU
|    Dt32        - time delta between r3 and r2   TU
|    p           - P vector    r2 x r3
|    Pn          - P Unit Vector
|    R1N         - R1 Unit Vector
|    Theta1      - temporary ANGLE between vec    rad
|    TolAngle    - Tolerance ANGLE  (1 deg)       rad
|    Term1       - 1st Term for HGibbs expansion
|    Term2       - 2nd Term for HGibbs expansion
|    Term3       - 3rd Term for HGibbs expansion
|    i           - Index
|
|  Coupling      :
|    MAG         - Magnitude of a vector
|    CROSS       - CROSS product of two vectors
|    DOT         - DOT product of two vectors
|    ASIN      - Arc sine FUNCTION
|    NORM        - Creates a Unit Vector
|    LNCOM3      - Combination of three scalars and three vectors
|    ANGLE       - ANGLE between two vectors
|
|  References    :
|    Vallado       2001, 439-445, Alg 52, Ex 7-4
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE HERRGIBBS          ( R1,R2,R3                              : Vector;
                               JD1,JD2,JD3                           : EXTENDED;
                               VAR V2                                : Vector;
                               VAR Theta,Theta1,Copa                 : EXTENDED;
                               VAR Error                             : Str12 );
   CONST
     TUDay : EXTENDED = 0.00933809017716;
   VAR
     i            : INTEGER;
     p, Pn, R1n   : Vector;
     Dt21, Dt31, Dt32, Term1, Term2, Term3, TolAngle : EXTENDED;
   BEGIN
     { --------------------  Initialize values   -------------------- }
     Error:=  'ok';
     Theta:=  0.0; Theta1:= 0.0;
     MAG( R1 );
     MAG( R2 );
     MAG( R3 );
     FOR i:= 1 to 4 DO
         V2[i]:= 0.0;
     TolAngle:= 0.01745329251994;
     Dt21:= (JD2-JD1)*86400.0;
     Dt31:= (JD3-JD1)*86400.0;   { differences in times }
     Dt32:= (JD3-JD2)*86400.0;

     { ----------------------------------------------------------------
     |  Determine IF the vectors are coplanar.
       -------------------------------------------------------------- }
     CROSS( R2,R3,P );
     NORM( P,Pn );
     NORM( R1,R1N );
     Copa:=  ASIN( DOT( Pn,R1n ) );
     IF ABS( DOT(R1N,Pn) ) > 0.017452406 THEN
         Error:= 'not coplanar';

     { ----------------------------------------------------------------
     | Check the size of the angles between the three position vectors.
     |   Herrick GIBBS only gives "reasonable" answers when the
     |   position vectors are reasonably close.  10 deg is only an estimate.
       -------------------------------------------------------------- }
     ANGLE( R1,R2, Theta );
     ANGLE( R2,R3, Theta1 );
     IF (Theta > TolAngle) or (Theta1 > TolAngle) THEN
         Error:= 'ANGLE > 1ø';

     { ------------ Perform Herrick-GIBBS method to find V2 --------- }
     Term1:= -Dt32*( 1.0/(Dt21*Dt31) + 3.986004418e5/(12.0*r1[4]*r1[4]*r1[4]) );
     Term2:= (Dt32-Dt21)*( 1.0/(Dt21*Dt32) + 3.986004418e5/(12.0*r2[4]*r2[4]*r2[4]) );
     Term3:=  Dt21*( 1.0/(Dt32*Dt31) + 3.986004418e5/(12.0*r3[4]*r3[4]*r3[4]) );
     LNCOM3( Term1,Term2,Term3,R1,R2,R3, V2 );

   END; { PROCEDURE HERRGIBBS }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE LAMBERTUNIV
|
|  This PROCEDURE solves the Lambert problem for orbit determination and returns
|    the velocity vectors at each of two given position vectors.  The solution
|    uses Universal Variables for calculation and a bissection technique for
|    updating psi.
|
|  Algorithm     : Setting the initial bounds:
|                  Using -8Pi and 4Pi2 will allow single rev solutions
|                  Using -4Pi2 and 8Pi2 will allow multi-rev solutions
|                  The farther apart the initial guess, the more iterations
|                    because of the iteration
|                  Inner loop is for special cases. Must be sure to exit both!
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    R1          - IJK Position vector 1          ER
|    R2          - IJK Position vector 2          ER
|    DM          - direction of motion            'L','S'
|    DtTU        - Time between R1 and R2         TU
|
|  OutPuts       :
|    V1          - IJK Velocity vector            ER / TU
|    V2          - IJK Velocity vector            ER / TU
|    Error       - Error flag                     'ok', ...
|
|  Locals        :
|    VarA        - Variable of the iteration,
|                  NOT the semi or axis!
|    Y           - Area between position vectors
|    Upper       - Upper bound for Z
|    Lower       - Lower bound for Z
|    CosDeltaNu  - Cosine of true anomaly change  rad
|    F           - f expression
|    G           - g expression
|    GDot        - g DOT expression
|    XOld        - Old Universal Variable X
|    XOldCubed   - XOld cubed
|    ZOld        - Old value of z
|    ZNew        - New value of z
|    C2New       - C2(z) FUNCTION
|    C3New       - C3(z) FUNCTION
|    TimeNew     - New time                       TU
|    Small       - Tolerance for roundoff errors
|    i, j        - index
|
|  Coupling      :
|    MAG         - Magnitude of a vector
|    DOT         - DOT product of two vectors
|    FINDC2C3    - Find C2 and C3 functions
|
|  References    :
|    Vallado       2001, 459-464, Alg 55, Ex 7-5
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE LAMBERTUNIV        ( ro,r                                  : Vector;
                               dm,OverRev                            : CHAR;
                               DtTU                                  : EXTENDED;
                               VAR vo,v                              : Vector;
                               VAR Error                             : Str12 );
   CONST
      TwoPi    : EXTENDED = 2.0 * pi;  { 6.28318530717959; }
      Small    : EXTENDED = 0.0000001;
      NumIter  : INTEGER  = 40;
   VAR
      Loops, i, YNegKtr : INTEGER;
      VarA, Y, Upper, Lower, CosDeltaNu, F, G, GDot, XOld, XOldCubed, FDot,
      PsiOld, PsiNew, C2New, C3New, dtNew  : EXTENDED;
   BEGIN
     { --------------------  Initialize values   -------------------- }
     Error  := 'ok';
     PsiNew := 0.0;
     MAG(ro);
     MAG(r);
     FOR i:= 1 to 4 DO
         vo[i]:= 0.0;
     v:= vo;

     CosDeltaNu:= DOT(ro,r)/(ro[4]*r[4]);
     IF Dm = 'L' THEN
         VarA := -SQRT( ro[4]*r[4]*(1.0+CosDeltaNu) )
       ELSE
         VarA :=  SQRT( ro[4]*r[4]*(1.0+CosDeltaNu) );

     { ----------------  Form Initial guesses   --------------------- }
     PsiOld := 0.0;
     PsiNew := 0.0;
     xOld   := 0.0;
     C2New  := 0.5;
     C3New  := 1.0/6.0;

     { -------- Set up initial bounds for the bissection ------------ }
     IF OverRev = 'N' THEN
       BEGIN
         Upper:= TwoPi*TwoPi;
         Lower:= -4.0*TwoPi;
       END
       ELSE
       BEGIN  { Multi-revs }
         Upper:= -0.001+4.0*TwoPi*TwoPi;  { at 4, not alw work   }
         Lower:=  0.001+TwoPi*TwoPi;      { 2.0, makes orbit bigger }
       END;                               { how about 2 revs??xx }

     { --------  Determine IF the orbit is possible at all ---------- }
     IF ABS( VarA ) > Small THEN
       BEGIN
{}
         Loops  := 0;
         YNegKtr:= 1; { y neg ktr }
         REPEAT
             IF ABS(C2New) > Small THEN
                 Y:= ro[4] + r[4] - ( VarA*(1.0-PsiOld*C3New)/SQRT(C2New) )
               ELSE
                 Y:= ro[4] + r[4];
             { ------- Check for negative values of y ------- }
             IF ( VarA > 0.0 ) and ( Y < 0.0 ) THEN
               BEGIN
                 YNegKtr:= 1;
                 REPEAT
                     PsiNew:= 0.8*(1.0/C3New)*( 1.0 - (ro[4]+r[4])*SQRT(C2New)/VarA  );
                     { ------ Find C2 and C3 functions ------ }
                     FINDC2C3( PsiNew, C2New,C3New );
                     PsiOld:= PsiNew;
                     Lower:= PsiOld;
                     IF ABS(C2New) > Small THEN
                         Y:= ro[4] + r[4] - ( VarA*(1.0-PsiOld*C3New)/SQRT(C2New) )
                       ELSE
                         Y:= ro[4] + r[4];
                     IF Show = 'Y' THEN
                       begin
                         WriteLn(YNegKtr:3,' y PsiNew ',y:11:7,PsiNew:11:7 );
                         readln;
                       end;
                     INC( YNegKtr );
                 UNTIL ( Y >= 0.0 ) or ( YNegKtr >= 10 );
               END; { IF y neg }
             IF YNegKtr < 10 THEN
               BEGIN
                 IF ABS(C2New) > Small THEN
                     XOld:= SQRT( Y/C2New )
                   ELSE
                     XOld:= 0.0;
                 XOldCubed:= XOld*XOld*XOld;
                 dtNew    := XOldCubed*C3New + VarA*SQRT(Y);

                 { ----  Readjust upper and lower bounds ---- }
                 IF dtNew < DtTU THEN
                     Lower:= PsiOld;
                 IF dtNew > DtTU THEN
                     Upper:= PsiOld;
                 PsiNew:= (Upper+Lower) * 0.5;

                 IF Show = 'Y' THEN
                   begin
                     WriteLn( Loops:2,PsiOld:10:5,Y:10:5,XOld:10:5,
                                      dtNew:10:5,VarA:7:3,upper:9:5,lower:9:5 );
                     readln;
                   end;

                 { -------------- Find c2 and c3 functions ---------- }
                 FINDC2C3( PsiNew, C2New,C3New );
                 PsiOld := PsiNew;
                 INC( Loops );

                 { ---- Make sure the first guess isn't too close --- }
                 IF (ABS(dtNew - DtTU) < Small) and (Loops = 1) THEN
                     dtNew:= DtTU-1.0;
               END; { IF YNegKtr < 10 }
         UNTIL (ABS(dtNew-DtTU) < Small) or (Loops > NumIter) or (YNegKtr > 10);

         IF (Loops >= NumIter) or (YNegKtr >= 10) THEN
           BEGIN
             Error:= 'GNotConv';
             IF YNegKtr >= 10 THEN
                 Error:= 'Y negative';
           END
           ELSE
           BEGIN
             { ---- Use F and G series to find Velocity Vectors ----- }
             F   := 1.0 - Y/ro[4];
             GDot:= 1.0 - Y/r[4];
             G   := 1.0 / (VarA*SQRT( Y )); { 1 over G }
             FDot:= SQRT(y)*(-r[4]-ro[4]+y)/(r[4]*ro[4]*VarA);
             FOR i:= 1 to 3 DO
               BEGIN
                 vo[i]:= ( r[i] - F*ro[i] )*G;
                 v[i] := ( GDot*r[i] - ro[i] )*G;
               END;
             MAG( vo );
             MAG( v  );
           END;  { IF the answer has converged }
       END  { IF Var A > 0.0 }
       ELSE
         Error:= 'impos 180ø';

    { ---- For Fig 6-14 dev with testgau.pas ---- }
{    IF Error = 'ok' THEN Write( FileOut,PsiNew:14:7,DtTU*13.44685:14:7 )
      ELSE Write( FileOut,' 9999.0 ':14,DtTU*13.44685:14:7 );}
   END;  { PROCEDURE LAMBERTUNIV }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE LAMBERBATTIN
|
|  This PROCEDURE solves Lambert's problem using Battins method. The method is
|    developed in Battin (1987).
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Ro          - IJK Position vector 1          ER
|    R           - IJK Position vector 2          ER
|    DM          - direction of motion            'L','S'
|    DtTU        - Time between R1 and R2         TU
|
|  OutPuts       :
|    Vo          - IJK Velocity vector            ER / TU
|    V           - IJK Velocity vector            ER / TU
|    Error       - Error flag                     'ok',...
|
|  Locals        :
|    i           - Index
|    Loops       -
|    u           -
|    b           -
|    Sinv        -
|    Cosv        -
|    rp          -
|    x           -
|    xn          -
|    y           -
|    l           -
|    m           -
|    CosDeltaNu  -
|    SinDeltaNu  -
|    DNu         -
|    a           -
|    Tan2w       -
|    RoR         -
|    h1          -
|    h2          -
|    Tempx       -
|    eps         -
|    denom       -
|    chord       -
|    k2          -
|    s           -
|    f           -
|    g           -
|    fDot        -
|    am          -
|    ae          -
|    be          -
|    tm          -
|    gDot        -
|    arg1        -
|    arg2        -
|    tc          -
|    AlpE        -
|    BetE        -
|    AlpH        -
|    BetH        -
|    DE          -
|    DH          -
|
|  Coupling      :
|    ASIN      - Arc sine FUNCTION
|    ACOS      - Arc cosine FUNCTION
|    MAG         - Magnitude of a vector
|    ASINH     - Inverse hyperbolic sine
|    ACOSH     - Inverse hyperbolic cosine
|    SINH        - Hyperbolic sine
|    POWER       - Raise a base to a POWER
|    ATAN2       - Arc tangent FUNCTION that resolves quadrants
|
|  References    :
|    Vallado       2001, 464-467, Ex 7-5
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE LAMBERTBATTIN      ( ro,r                                  : Vector;
                               dm,OverRev                            : CHAR;
                               DtTU                                  : EXTENDED;
                               VAR vo,v                              : Vector;
                               VAR Error                             : Str12 );
   CONST
     Small      : EXTENDED =     0.000001;
   VAR
     i, Loops : INTEGER;
     RCrossR  : Vector;
     {    lambda,bigt,     testamt,}
     u, b, Sinv,Cosv, rp, x, xn, y, l, m, CosDeltaNu, SinDeltaNu,DNu, a,
     tan2w, RoR, h1, h2, tempx, eps, denom, chord, k2, s, f, g, fDot, am,
     ae, be, tm, gDot, arg1, arg2, tc, AlpE, BetE, AlpH, BetH, DE, DH : EXTENDED;

   FUNCTION SEE                ( v                         : EXTENDED ) : EXTENDED;
     VAR
       c : array[0..20] of EXTENDED;
       term, termold, del, delold, sum1, temp, eta, sum, SQRTopv : EXTENDED;
       i : INTEGER;
     BEGIN
       c[0] :=    0.2;
       c[1] :=    9.0 /  35.0;        c[2] :=   16.0 /  63.0;
       c[3] :=   25.0 /  99.0;        c[4] :=   36.0 / 143.0;
       c[5] :=   49.0 / 195.0;        c[6] :=   64.0 / 255.0;
       c[7] :=   81.0 / 323.0;        c[8] :=  100.0 / 399.0;
       c[9] :=  121.0 / 483.0;        c[10]:=  144.0 / 575.0;
       c[11]:=  169.0 / 675.0;        c[12]:=  196.0 / 783.0;
       c[13]:=  225.0 / 899.0;        c[14]:=  256.0 /1023.0;
       c[15]:=  289.0 /1155.0;        c[16]:=  324.0 /1295.0;
       c[17]:=  361.0 /1443.0;        c[18]:=  400.0 /1599.0;
       c[19]:=  441.0 /1763.0;        c[20]:=  484.0 /1935.0;
       SQRTOpv:= SQRT(1.0 + v);
       eta    := v / SQR( 1.0+SQRTOpv );

       { ---- Process Forwards ---- }
       delold := 1.0;
       termold:= c[0]{ * eta};
       sum1   := termold;
       i:= 1;
       WHILE (i <= 20) and (ABS(Termold) > 0.000001 ) DO
         BEGIN
           del  := 1.0 / ( 1.0 + c[i]*eta*delold );
           term := termold * (del - 1.0);
           sum1 := sum1 + term;
           INC(i);
           delold := del;
           termold:= term;
         END;

       See:= (1.0 / (8.0*(1.0+SQRTOpv))) * ( 3.0 + Sum1 / ( 1.0+eta*sum1 ) );
     END;  { Internal FUNCTION See }

   FUNCTION k                  ( v                         : EXTENDED ) : EXTENDED;
     VAR
       d : array[0..20] of EXTENDED;
       del,delold,term,termold,  temp,       Sum,sum1 : EXTENDED;
       i : INTEGER;
     BEGIN
       d[0] :=     1.0 /    3.0;        d[1] :=     4.0 /   27.0;
       d[2] :=     8.0 /   27.0;        d[3] :=     2.0 /    9.0;
       d[4] :=    22.0 /   81.0;        d[5] :=   208.0 /  891.0;
       d[6] :=   340.0 / 1287.0;        d[7] :=   418.0 / 1755.0;
       d[8] :=   598.0 / 2295.0;        d[9] :=   700.0 / 2907.0;
       d[10]:=   928.0 / 3591.0;        d[11]:=  1054.0 / 4347.0;
       d[12]:=  1330.0 / 5175.0;        d[13]:=  1480.0 / 6075.0;
       d[14]:=  1804.0 / 7047.0;        d[15]:=  1978.0 / 8091.0;
       d[16]:=  2350.0 / 9207.0;        d[17]:=  2548.0 /10395.0;
       d[18]:=  2968.0 /11655.0;        d[19]:=  3190.0 /12987.0;
       d[20]:=  3658.0 /14391.0;

       { ---- Process Forwards ---- }
       sum1   := d[0];
       delold := 1.0;
       termold:= d[0];
       i:= 1;
       WHILE (i <= 20) and (ABS(Termold) > 0.000001 ) DO
         BEGIN
           del  := 1.0 / ( 1.0 + d[i]*v*delold );
           term := termold * ( del - 1.0 );
           sum1 := sum1 + term;
           INC(i);
           delold := del;
           termold:= term;
         END;

       k:= Sum1;
   END; { Internal PROCEDURE K }
{}
   BEGIN
     { --------------------  Initialize values   -------------------- }
     Error:= 'ok';
     CosDeltaNu:= DOT(ro,r)/(ro[4]*r[4]);
     CROSS( ro,r, RCrossR );
     SinDeltaNu:= RCrossR[4]/(Ro[4]*r[4]);
     DNu:= ATAN2( SinDeltaNu,CosDeltaNu );

     RoR  := r[4]/ro[4];
     eps  := RoR - 1.0;
     tan2w:= 0.25*eps*eps / ( SQRT( RoR ) + RoR *( 2.0 + SQRT( RoR ) ) );
     rp   := SQRT( ro[4]*r[4] )*( SQR( COS(DNu*0.25) ) + tan2w );

     IF DNu < Pi THEN
         L := ( SQR( SIN(DNu*0.25) ) + tan2w ) /
              ( SQR( SIN(DNu*0.25) ) + tan2w + COS( DNu*0.5 ) )
       ELSE
         L := ( SQR( COS(DNu*0.25) ) + tan2w - COS( DNu*0.5 ) ) /
              ( SQR( COS(DNu*0.25) ) + tan2w );

     m    := DtTU*DtTU / ( 8.0*rp*rp*rp );
     xn   := 0.0{L};   { 0 for par and hyp }
     chord:= SQRT( ro[4]*ro[4] + r[4]*r[4] - 2.0*ro[4]*r[4]*COS( DNu ) );
     s    := ( ro[4] + r[4] + chord )*0.5;

     Loops:= 1;
     REPEAT
         x    := xn;
         Tempx:= See(x);
         Denom:= 1.0 / ( (1.0+2.0*x+L) * (3.0+x* (1.0+4.0*Tempx) ) );
         h1   := SQR( L+x )*( 1.0+ (1.0+3.0*x)*Tempx )*Denom;
         h2   := m*( 1.0 + (x - L)*Tempx )*Denom;

         { ------------------------ Evaluate CUBIC ------------------ }
         b := 0.25*27.0*h2 / POWER( 1.0+h1,3 );
         u := -0.5*b / ( 1.0 + SQRT( 1.0 + b ) );
         K2:= K(u);

         y := ( ( 1.0+h1 ) / 3.0 )*( 2.0 + SQRT( 1.0+b ) /
              ( 1.0-2.0*u*k2*k2 ) );
         xn:= SQRT( SQR( (1.0-L)*0.5 ) + m/(y*y) ) - ( 1.0+L )*0.5;

         INC(Loops);

     UNTIL (ABS(xn-x) < Small) or (Loops > 30);

     a:=  DtTU*DtTU / (16.0*Rp*rp*xn*y*y );
  {  a := rp*m / ( 2.0 * xn*y*y );}

     { -------------------- Find Eccentric anomalies ---------------- }
     { -------------------------- Hyperbolic ------------------------ }
     IF a < -Small THEN
       BEGIN
         arg1 := SQRT( s / ( -2.0*a ) );
         arg2 := SQRT( ( s-chord ) / ( -2.0*a ) );
         { -------- Evaluate f and g functions -------- }
         AlpH := 2.0 * ASINH( arg1 );
         BetH := 2.0 * ASINH( arg2 );
         DH   := AlpH - BetH;
         F    := 1.0 - (a/ro[4])*(1.0 - COSH(DH) );
         GDot := 1.0 - (a/r[4]) *(1.0 - COSH(DH) );
         G    := DtTU - SQRT(-a*a*a)*(SINH(DH)-DH);
         FDot := -SQRT(-a)*SINH(DH)/(ro[4]*r[4]);
       END
       ELSE
         { ------------------------- Elliptical --------------------- }
         IF a > small THEN
           BEGIN
             arg1 := SQRT( s / ( 2.0*a ) );
             arg2 := SQRT( ( s-chord ) / ( 2.0*a ) );
             Sinv := arg2;
             Cosv := SQRT( 1.0 - (ro[4]+r[4]-chord)/(4.0*a) );
             BetE := 2.0*ACOS(Cosv);
             BetE := 2.0*ASIN(Sinv);
             IF DNu > Pi THEN   { Quad check }
                 BetE:= -BetE;

             Cosv:= SQRT( 1.0 - s/(2.0*a) );
             Sinv:= arg1;

             am  := s*0.5;
             ae  := Pi;
             be  := 2.0*ASIN( SQRT( (s-chord)/s ) );
             tm  := SQRT(am*am*am)*(ae - (be-SIN(be)));
             IF DtTU > tm THEN
                 AlpE:= 2.0*pi-2.0*ASIN( Sinv )
               ELSE
                 AlpE:= 2.0*ASIN( Sinv );
             DE  := AlpE - BetE;
             F   := 1.0 - (a/ro[4])*(1.0 - COS(DE) );
             GDot:= 1.0 - (a/r[4])* (1.0 - COS(DE) );
             G   := DtTU - SQRT(a*a*a)*(DE - SIN(DE));
             FDot:= -SQRT(a)*SIN(DE)/(ro[4]*r[4]);
           END
           ELSE
           { ------------------------- Parabolic -------------------- }
           BEGIN
             arg1 := 0.0;
             arg2 := 0.0;
             Error:= 'a = 0 ';
             WriteLn(FileOut,' a parabolic orbit ' );
           END;

     FOR i:= 1 to 3 DO
       BEGIN
         vo[i]:= ( r[i] - F*ro[i] )/G;
         v[i] := ( GDot*r[i] - ro[i] )/G;
       END;
     MAG( vo );
     MAG( v  );

 {     IF Error = 'ok' THEN
         Testamt:= F*GDot - FDot*G
       ELSE
         TestAmt:= 2.0;
     write( FileOut,TestAmt:8:5,Loops:3 );
 }
 { WriteLn( 'l ',l );}
 { WriteLn( 'rp,L,m,x ',rp:11:7,L:11:7,m:11:7,xn:11:7 );}
 { WriteLn( 'chord ',chord:11:7,' s ',s:11:7 );}
 { WriteLn( 'h1          h2              b          y             x          a ' );}
 { WriteLn( 'h1 h2 ',h1:11:7, h2:11:7 );}
 { WriteLn ('k2 ',k2:11:7,' y',y:11:7 );  WriteLn ('x ',xn:11:7 );}
 { WriteLn( Loops:3,h1:11:7,h2:11:7,b:11:7,y:11:7,x:11:7 );}
 {WriteLn( 'a',a:11:7 );}
 {     Lambda:= -SQRT( (s-Chord)/s );
     BigT:= SQRT( 8.0/(s*s*s) )*DtTU;
 }
   END;  { PROCEDURE LAMBERTBATTIN }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE TARGET
|
|  This PROCEDURE accomplishes the targeting problem using KEPLER/PKEPLER and
|    Lambert.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    RInt        - Initial Position vector of Int ER
|    VInt        - Initial Velocity vector of Int ER/TU
|    RTgt        - Initial Position vector of Tgt ER
|    VTgt        - Initial Velocity vector of Tgt ER/TU
|    dm          - Direction of Motion for Gauss  'L','S'
|    Kind        - Type of propagator             'K','P'
|    DtTU        - Time of flight to the int      TU
|
|  Outputs       :
|    V1t         - Initial Transfer Velocity vec  ER/TU
|    V2t         - Final Transfer Velocity vec    ER/TU
|    DV1         - Initial Change Velocity vec    ER/TU
|    DV2         - Final Change Velocity vec      ER/TU
|    Error       - Error flag from Gauss          'ok', ...
|
|  Locals        :
|    TransNormal - CROSS product of trans orbit   ER
|    IntNormal   - CROSS product of int orbit     ER
|    R1Tgt       - Position vector after Dt, Tgt  ER
|    V1Tgt       - Velocity vector after Dt, Tgt  ER/TU
|    RIRT        - RInt[4] * R1Tgt[4]
|    CosDeltaNu  - Cosine of DeltaNu              rad
|    SinDeltaNu  - Sine of DeltaNu                rad
|    DeltaNu     - ANGLE between position vectors rad
|    i           - Index
|
|  Coupling      :
|    KEPLER      - Find R and V at future time
|    LAMBERTUNIV - Find velocity vectors at each END of transfer
|    LNCOM2      - Linear combination of two vectors and constants
|
|  References    :
|    Vallado       2001, 468-474, Alg 58
|
 ----------------------------------------------------------------------------- }

PROCEDURE TARGET             ( RInt,VInt,RTgt,VTgt                   : Vector;
                               Dm,Kind                               : CHAR;
                               DtTU                                  : EXTENDED;
                               VAR V1t,V2t,DV1,DV2                   : Vector;
                               VAR Error                             : Str12 );
   VAR
     IntNormal, TransNormal, R1Tgt, V1Tgt         : Vector;
     Temp, RIRT, CosDeltaNu, SinDeltaNu, DeltaNu  : EXTENDED;
     i : INTEGER;
   BEGIN
     { ----------- Propogate TARGET forward by time ----------------- }
     CASE Kind OF
       'K' : KEPLER ( RTgt,VTgt,DtTU,  R1Tgt,V1Tgt,Error );
{       'P' : PKEPLER( RTgt,VTgt,DtTU,  R1Tgt,V1Tgt );} {}{}{}{}{}
     END;  { Case }

     { ----------- Calculate transfer orbit between r's ------------- }
     IF Error = 'ok' THEN
       BEGIN
         LAMBERTUNIV( RInt,R1Tgt,dm,'N',DtTU,  V1t,V2t,Error );

         IF Error = 'ok' THEN
           BEGIN
             LNCOM2( -1.0, 1.0,VInt, V1t,  DV1 );
             LNCOM2(  1.0,-1.0,V1Tgt,V2t,  DV2 );
           END
           ELSE
           BEGIN
             FOR i:= 1 to 4 DO
                 V1t[i]:= 0.0;
             V2t:= V1t;
             DV1:= V1t;
             DV2:= V1t;
           END;
       END;
   END;  { PROCEDURE TARGET }

BEGIN

END.  { Unit AstIOD }
{}
