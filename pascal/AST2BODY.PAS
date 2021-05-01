(*     ----------------------------------------------------------------      *)

                               UNIT AST2BODY;

(*   this file contains fundamental astrodynamic procedures and functions
*   using 2-body dynamics. the routines span a wide range of material, and
*   they come from chapters 2, 3, 5, and 11.
*
*                            companion code for
*               fundamentals of astrodynamics and applications
*                                    2004
*                              by david vallado
*
*       (w) 719-573-2600, email dvallado@agi.com
*
*    current :
*              30 may 07  david vallado
*                           3rd edition baseline
*    changes :
*              21 jul 05  david vallado
*                           2nd printing baseline
*              14 May 01  David Vallado                                       
*                           2nd edition baseline                              
*              23 Nov 87  David Vallado                                       
*                           Original Baseline                                 
*                                                                           
*      ----------------------------------------------------------------      *)

                                  INTERFACE

(*     ----------------------------------------------------------------      *)

   Uses
         AstMath,
         AstTime;


PROCEDURE rv2coe             ( R,V                                   : Vector;
                               VAR P,A,Ecc,Incl,Omega,Argp,
                               Nu,M,ArgLat,TrueLon,LonPer            : EXTENDED );

PROCEDURE coe2rv             ( P,Ecc,Incl,Omega,Argp,Nu,
                               ArgLat,TrueLon,LonPer                 : EXTENDED;
                               VAR R,V                               : Vector   );

PROCEDURE FINDC2C3           ( ZNew                                  : EXTENDED;
                               VAR C2New,C3New                       : EXTENDED );

PROCEDURE NEWTONE            ( Ecc,E0                                : EXTENDED;
                               VAR M,Nu                              : EXTENDED );

PROCEDURE NEWTONM            ( Ecc,M                                 : EXTENDED;
                               VAR E0,Nu                             : EXTENDED );

PROCEDURE NEWTONNU           ( Ecc,Nu                                : EXTENDED;
                               VAR E0,M                              : EXTENDED );

PROCEDURE KEPLER             ( Ro,Vo                                 : Vector;
                               Dtsec                                 : EXTENDED;
                               VAR R,V                               : Vector;
                               VAR Error                             : Str12    );

PROCEDURE FINDTOF            ( Ro,R                                  : Vector;
                               p                                     : EXTENDED;
                               VAR Tof                               : EXTENDED );

PROCEDURE ijk2llA            ( R                                     : Vector;
                               JD                                    : EXTENDED;
                               VAR Latgc,Latgd,Lon,Hellp             : EXTENDED );

PROCEDURE ijk2llE            ( R                                     : Vector;
                               JD                                    : EXTENDED;
                               VAR Latgc,Latgd,Lon,Hellp             : EXTENDED );

PROCEDURE ijk2llB            ( R                                     : Vector;
                               JD                                    : EXTENDED;
                               VAR Latgc,Latgd,Lon,Hellp             : EXTENDED );

PROCEDURE gc2gd              ( VAR Latgc                             : EXTENDED;
                               Direction                             : Direct;
                               VAR Latgd                             : EXTENDED );

PROCEDURE SIGHT              ( R1,R2                                 : Vector;
                               WhichKind                             : CHAR;
                               VAR LOS                               : Str3     );

PROCEDURE SUN                ( JD                                    : EXTENDED;
                               VAR RSun                              : Vector;
                               VAR RtAsc,Decl                        : EXTENDED );

PROCEDURE SunIll             ( JD, Lat, Lon                          : EXTENDED;
                               VAR SunIll,SunAz,SunEl                : EXTENDED );

PROCEDURE MOON               ( JD                                    : EXTENDED;
                               VAR RMoon                             : Vector;
                               VAR RtAsc,Decl                        : EXTENDED );

PROCEDURE MoonIll            ( MoonEl,f                              : EXTENDED;
                               VAR MoonIll                           : Extended );

PROCEDURE LIGHT              ( R                                     : Vector;
                               JD                                    : EXTENDED;
                               WhichKind                             : CHAR;
                               VAR LIT                               : Str3     );

PROCEDURE CHECKHITEARTH      ( Rint,V1t,Rtgt,V2t                     : Vector;
                               VAR HitEarth                          : CHAR     );

PROCEDURE SATFOV             ( Incl,Az, SLatgd, SLon, SAlt, tFOV,
                               EtaCtr                                : EXTENDED;
                               VAR FovMax, TotalRng, RhoMax, RhoMin,
                               TgtLat, TgtLon                        : EXTENDED );

PROCEDURE RNGAZ              ( LLat,LLon,TLat,TLon,Tof               : EXTENDED;
                               VAR Range, Az                         : EXTENDED );

PROCEDURE PATH               ( LLat, LLon, Range, Az                 : EXTENDED;
                               VAR TLat, TLon                        : EXTENDED );


{ ------------------------ Constants used in this File ------------------------
|
|     Small      : EXTENDED =     0.00000001;        Small value for tolerances
|     HalfPi     : EXTENDED =     1.57079632679490;
|     Pi         : EXTENDED =     3.14159265358979;
|     TwoPi      : EXTENDED =     6.28318530717959;
|     Infinite   : EXTENDED = 999999.9;              Infinite value
|     Undefined  : EXTENDED = 999999.1;              Undefined value
|     Rad2Deg    : EXTENDED =    57.29577951308230;
|     Deg2Rad    : EXTENDED =     0.01745329251994;
|     EESqrd     : EXTENDED =     0.006694385000;    Eccentricity of Earth Sqrd
|     OmegaEarth : EXTENDED =     0.05883359221938136;  Earth Rot rad/TU
|
 ----------------------------------------------------------------------------- }

(*     -----------------------------------------------------------------      *)

                                IMPLEMENTATION

(*     -----------------------------------------------------------------      *)
{}
(*

{         Sinv:= ( Ecc * SIN(Nu) ) / SQRT( 1.0+ 2.0*Ecc*COS(Nu) + Ecc*Ecc );}
         he:= 1.0+ 2.0*Ecc*COS(Nu) + Ecc*Ecc;
         IF He > 0.000001 THEN
             Cosv:= ( 1.0 + Ecc * COS(Nu) ) / SQRT(he)
           ELSE
             Cosv:= 0.0;  { Fix for special case (from testgau) }

         fpa:= ACOS( Cosv );

         ANGLE( r,v  fpa );


         { ------------------ Calculate equinoctial elements ----------------- }
Vallado   2001,

         Fr:= 1.0;
         IF Incl > HalfPi THEN
             Fr:= -1.0;
         he:= Ecc*COS(Fr*Omega + Argp);
         ke:= Ecc*SIN(Fr*Omega + Argp);
         IF a >= 0.0 THEN
           BEGIN
             IF a < 9999.0 THEN
                 n:= SQRT(1.0/(a*a*a))
               ELSE
                 n:= 0.0;
           END
           ELSE
             n:= SQRT(1.0/(-a*a*a));

         LamMean:= M + Fr*Omega + Argp;

         pe:= POWER( TAN(incl*0.5),Fr )*SIN(Omega);
         qe:= POWER( TAN(incl*0.5),Fr )*COS(Omega);

{}           WriteLn( FileOut,'fpa     ',fpa*57.29578:14:7 );
{}           WriteLn( FileOut,'Equinoc k h n ',ke:11:7,he:11:7,n:11:7 );
{}           WriteLn( FileOut,'Equinoc l p q ',lammean:11:7,pe:11:7,qe:11:7 );
*)
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE rv2coe
|
|  This PROCEDURE finds the classical orbital elements given the Geocentric
|    Equatorial Position and Velocity vectors.  Special cases for equatorial
|    and circular orbits are also handled.  IF the elements are Infinite, they
|    are set to 999999.9. IF elements are Undefined, they are set to 999999.1.
|    Be sure to check for these during output or subsequent use!!
|
|  Algorithm     : Initialze variables
|                  IF the HBar magnitude exists, contiNue, otherwise exit and
|                       assign undefined values
|                    Find vectors and values
|                    Determine the type of orbit with IF statements
|                    Find angles depending on the orbit type
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    R           - IJK Position vector            km
|    V           - IJK Velocity vector            km / s
|
|  Outputs       :
|    P           - SemiLatus rectum               km
|    A           - semimajor axis                 km
|    Ecc         - Eccentricity
|    Incl        - inclination                    0.0 to Pi rad
|    Omega       - Longitude of Ascending Node    0.0 to 2Pi rad
|    Argp        - Argument of Perigee            0.0 to 2Pi rad
|    Nu          - True anomaly                   0.0 to 2Pi rad
|    M           - Mean anomaly                   0.0 to 2Pi rad
|    ArgLat      - Argument of Latitude      (CI) 0.0 to 2Pi rad
|    LamTrue     - True Longitude            (CE) 0.0 to 2Pi rad
|    LonPer      - Longitude of Periapsis    (EE) 0.0 to 2Pi rad
|
|  Locals        :
|    HBar        - Angular Momentum H Vector      km2 / s
|    EBar        - Eccentricity     E Vector
|    NBar        - Line of Nodes    N Vector
|    c1          - V**2 - u/R
|    RDotV       - R DOT V
|    Hk          - Hk unit vector
|    SME         - Specfic Mechanical Energy      km2 / s2
|    i           - index
|    E           - Eccentric, Parabolic,
|                  Hyperbolic Anomaly             rad
|    Temp        - Temporary variable
|    TypeOrbit   - Type of orbit                  EE, EI, CE, CI
|
|  Coupling      :
|    MAG         - Magnitude of a vector
|    CROSS       - CROSS product of two vectors
|    DOT         - DOT product of two vectors
|    ACOS      - Arc Cosine FUNCTION
|    ANGLE       - Find the ANGLE between two vectors
|    NEWTONNU    - Find the mean anomaly
|    MODFUNC     - MOD FUNCTION for REAL variables
|
|  References    :
|    Vallado       2007, 121, Alg 9, Ex 2-5
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE rv2coe              ( R,V                                   : Vector;
                               VAR P,A,Ecc,Incl,Omega,Argp,
                               Nu,M,ArgLat,TrueLon,LonPer            : EXTENDED );
   CONST
     TwoPi      : EXTENDED =     2.0 * pi;  { 6.28318530717959; }
     HalfPi     : EXTENDED =     0.5 * pi;  { 1.57079632679490; }
     Small      : EXTENDED =     0.00000001;       { Small value for tolerances }
     Infinite   : EXTENDED = 999999.9;             { Infinite value  }
     Undefined  : EXTENDED = 999999.1;             { Undefined value }
     Mu         : EXTENDED = 398600.4418;          { graviational parameter }
   VAR
     i                  : INTEGER;
     TypeOrbit          : String[2];
     HBar, EBar, NBar   : Vector;
     E, c1, RDotV, Hk, SME, Temp : EXTENDED;
   BEGIN
     { --------------------  Initialize values   -------------------- }
     MAG( R );
     MAG( V );
     { -------------------  Find H N and E vectors   ---------------- }
     CROSS( R,V,HBar );
     IF HBar[4] > Small THEN
       BEGIN
         NBar[1]:= -HBar[2];
         NBar[2]:=  HBar[1];
         NBar[3]:=   0.0;
         MAG( NBar );
         c1 := V[4]*V[4] - Mu/R[4];
         RDotV:= DOT( R,V );
         FOR i:= 1 to 3 DO
             EBar[i]:= (c1*R[i] - RDotV*V[i]) / Mu;
         MAG( EBar );

         { -------------  Find a e and semi-Latus rectum   ---------- }
         SME:= ( V[4]*V[4]*0.5 ) - ( Mu/R[4] );
         IF ABS( SME ) > Small THEN
             A:= -Mu / (2.0*SME)
           ELSE
             A:= Infinite;       { Parabola }
         Ecc:= EBar[4];
         P := HBar[4]*HBar[4] / Mu;

         { ------------------  Find inclination   ------------------- }
         Hk:= HBar[3]/HBar[4];
         IF ABS( ABS(Hk) - 1.0 ) < Small THEN
             { -------  Equatorial Orbits   ---------- }
             IF ABS(HBar[3]) > 0.0 THEN
                 Hk:= SGN(HBar[3]) * 1.0;
         Incl:= ACOS( Hk );

         { ---------  Determine type of orbit for Later use  -------- }
         { ------- Elliptical, Parabolic, Hyperbolic Inclined ------- }
         TypeOrbit:= 'EI';
         IF Ecc < Small THEN
             { --------------  Circular Equatorial -------------- }
             IF ( Incl < Small ) or ( ABS(Incl-Pi) < Small ) THEN
                 TypeOrbit:= 'CE'
               ELSE
                 { ------------  Circular Inclined -------------- }
                 TypeOrbit:= 'CI'
           ELSE
             { -- Elliptical, Parabolic, Hyperbolic Equatorial -- }
             IF ( Incl < Small ) or ( ABS(Incl-Pi) < Small ) THEN
               TypeOrbit:= 'EE';

         { -----------  Find Longitude of Ascending Node ------------ }
         IF NBar[4] > Small THEN
           BEGIN
             Temp:= NBar[1] / NBar[4];
             IF ABS(Temp) > 1.0 THEN
                 Temp:= SGN(Temp) * 1.0;
             Omega:= ACOS( Temp );
             IF NBar[2] < 0.0 THEN
                 Omega:= TwoPi - Omega;
           END
           ELSE
             Omega:= Undefined;

         { ----------------- Find Argument of perigee --------------- }
         IF TypeOrbit = 'EI' THEN
           BEGIN
             ANGLE( NBar,EBar,  Argp );
             IF EBar[3] < 0.0 THEN
                 Argp:= TwoPi - Argp;
           END
           ELSE
             Argp:= Undefined;
{}
         { -------------  Find True Anomaly at Epoch    ------------- }
         IF TypeOrbit[1] = 'E' THEN
           BEGIN
             ANGLE( EBar,r,  Nu );
             IF RDotV < 0.0 THEN
                 Nu:= TwoPi - Nu;
           END
           ELSE
             Nu:= Undefined;

         { -----  Find Argument of Latitude - Circular Inclined ----- }
         IF TypeOrbit = 'CI' THEN
           BEGIN
             ANGLE( NBar,R,  ArgLat );
             IF R[3] < 0.0 THEN
                 ArgLat:= TwoPi - ArgLat;
           END
           ELSE
             ArgLat:= Undefined;

         { --- Find Longitude of Perigee - Elliptical Equatorial ---- }
         IF ( EBar[4] > Small ) and ( TypeOrbit = 'EE' ) THEN
           BEGIN
             Temp:= EBar[1]/EBar[4];
             IF ABS(Temp) > 1.0 THEN
                 Temp:= SGN(Temp) * 1.0;
             LonPer:= ACOS( Temp );
             IF EBar[2] < 0.0 THEN
                 LonPer:= TwoPi - LonPer;
             IF Incl > HalfPi THEN
                 LonPer:= TwoPi - LonPer;
           END
           ELSE
             LonPer:= Undefined;

         { ------- Find True Longitude - Circular Equatorial -------- }
         IF ( R[4] > Small ) and ( TypeOrbit = 'CE' ) THEN
           BEGIN
             Temp:= R[1]/R[4];
             IF ABS(Temp) > 1.0 THEN
                 Temp:= SGN(Temp) * 1.0;
             TrueLon:= ACOS( Temp );
             IF R[2] < 0.0 THEN
                 TrueLon:= TwoPi - TrueLon;
             IF Incl > HalfPi THEN
                 TrueLon:= TwoPi - TrueLon;
           END
           ELSE
             TrueLon:= Undefined;

         { ------------- Find Mean Anomaly for all orbits ----------- }
         NEWTONNU(Ecc,Nu, E,M );

{}       IF Show IN ['Y','S'] THEN
{}         BEGIN
{}           WriteLn( FileOut,'H = ':6,HBar[1]:13:7,HBar[2]:14:7,HBar[3]:14:7,HBar[4]:14:7 );
{}           WriteLn( FileOut,'N = ':6,NBar[1]:13:7,NBar[2]:14:7,NBar[3]:14:7,NBar[4]:14:7 );
{}           WriteLn( FileOut,'E = ':6,EBar[1]:13:7,EBar[2]:14:7,EBar[3]:14:7,EBar[4]:14:7 );
{}           WriteLn( FileOut,'SME=':6,SME:13:7,' ER2/TU2' );
{}           WriteLn( FileOut,'Anomaly ',E*57.29578:14:7,' TypeOrbit = ',TypeOrbit:3 );
{}         END;

      END  { IF HBar[4] > 0 the orbit is possible }
      ELSE
      BEGIN
        P    := Undefined;
        A    := Undefined;
        Ecc  := Undefined;
        Incl := Undefined;
        Omega:= Undefined;
        Argp := Undefined;
        Nu   := Undefined;
        M    := Undefined;
        ArgLat  := Undefined;
        TrueLon:= Undefined;
        LonPer := Undefined;
      END;

   END; { PROCEDURE rv2coe }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE coe2rv
|
|  This PROCEDURE finds the position and velocity vectors in Geocentric
|    Equatorial (IJK) system given the classical orbit elements. Notice that
|    p is used for calcuLations and that semi-major axis a, is not.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    P           - SemiLatus rectum               km
|    Ecc         - Eccentricity
|    Incl        - inclination                    0.0 to Pi rad
|    Omega       - Longitude of Ascending Node    0.0 to 2Pi rad
|    Argp        - Argument of Perigee            0.0 to 2Pi rad
|    Nu          - True anomaly                   0.0 to 2Pi rad
|    ArgLat      - Argument of Latitude      (CI) 0.0 to 2Pi rad
|    LamTrue     - True Longitude            (CE) 0.0 to 2Pi rad
|    LonPer      - Longitude of Periapsis    (EE) 0.0 to 2Pi rad
|
|  Outputs       :
|    R           - IJK Position vector            km
|    V           - IJK Velocity vector            km / s
|
|  Locals        :
|    Temp        - Temporary EXTENDED value
|    Rpqw        - PQW Position vector            km
|    Vpqw        - PQW Velocity vector            km / s
|    SinNu       - Sine of Nu
|    CosNu       - Cosine of Nu
|    TempVec     - PQW Velocity vector
|
|  Coupling      :
|    MAG         - Magnitude of a vector
|    ROT3        - Rotation about the 3rd axis
|    ROT1        - Rotation about the 1st axis
|
|  References    :
|    Vallado       2007, 126, Alg 10, Ex 2-5
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE coe2rv              ( P,Ecc,Incl,Omega,Argp,Nu,
                               ArgLat,TrueLon,LonPer                 : EXTENDED;
                               VAR R,V                               : Vector );
   CONST
     Small      : EXTENDED =     0.00000001;       { Small value for tolerances }
     Mu         : EXTENDED = 398600.4418;          { graviational parameter }
   VAR
     Rpqw, Vpqw, TempVec : Vector;
     Temp, SinNu, CosNu  : EXTENDED;
   BEGIN
     { --------------------------------------------------------------
     |  Determine what type of orbit is involved and set up the
     |  set up angles for the special cases.
       -------------------------------------------------------------- }
     IF Ecc < Small THEN
         { ------------  Circular Equatorial  ------------ }
         IF ( Incl < Small ) or ( ABS(Incl - Pi) < Small ) THEN
           BEGIN
             Argp := 0.0;
             Omega:= 0.0;
             Nu   := TrueLon;
           END
           ELSE
           { ----------  Circular Inclined  -------------- }
           BEGIN
             Argp:= 0.0;
             Nu  := ArgLat;
           END
       ELSE
         { ----------  Elliptical Equatorial  ------------ }
         IF ( Incl < Small ) or ( ABS(Incl - Pi) < Small ) THEN
           BEGIN
             Argp := LonPer;
             Omega:= 0.0;
           END;

     { -----------  Form PQW position and velocity vectors ---------- }
     CosNu:= COS(Nu);
     SinNu:= SIN(Nu);
     Temp := P / (1.0 + Ecc*CosNu);
     Rpqw[1]:= Temp*CosNu;
     Rpqw[2]:= Temp*SinNu;
     Rpqw[3]:=     0.0;
     IF ABS(p) < 0.0001 THEN { IF input is 0.0}
         p:= 0.0001;
     Vpqw[1]:=    -SinNu    * SQRT(mu/P);
     Vpqw[2]:=  (Ecc + CosNu) * SQRT(mu/P);
     Vpqw[3]:=      0.0;
     MAG( Rpqw );
     MAG( Vpqw );

     { ---------------  Perform transformation to IJK  -------------- }
     ROT3( Rpqw   , -Argp , TempVec );
     ROT1( TempVec, -Incl , TempVec );
     ROT3( TempVec, -Omega,   R     );

     ROT3( Vpqw   , -Argp , TempVec );
     ROT1( TempVec, -Incl , TempVec );
     ROT3( TempVec, -Omega,   V     );
   END; { PROCEDURE coe2rv }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE FINDC2C3
|
|  This PROCEDURE calculates the C2 and C3 functions for use in the Universal
|    Variable calcuLation of z.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|                                                                4 Feb 1992
|  Inputs          Description                    Range / Units
|    ZNew        - Z variable                     rad2
|
|  Outputs       :
|    C2New       - C2 FUNCTION value
|    C3New       - C3 FUNCTION value
|
|  Locals        :
|    SqrtZ       - Square root of ZNew
|
|  Coupling      :
|    SINH        - Hyperbolic Sine
|    COSH        - Hyperbolic Cosine
|
|  References    :
|    Vallado       2007, 71, Alg 1
|
 ----------------------------------------------------------------------------- }

PROCEDURE FINDC2C3           ( ZNew                                  : EXTENDED;
                               VAR C2New,C3New                       : EXTENDED );
   CONST
     Small      : EXTENDED =     0.00000001;       { Small value for tolerances }
   VAR
     SqrtZ : EXTENDED;
   BEGIN
     IF ZNew > Small THEN
       BEGIN
         SqrtZ := SQRT( ZNew );
         C2New := (1.0-COS( SqrtZ )) / ZNew;
         C3New := (SqrtZ-SIN( SqrtZ )) / ( SqrtZ*SqrtZ*SqrtZ );
       END
       ELSE
       BEGIN
         IF ZNew < -Small THEN
           BEGIN
             SqrtZ := SQRT( -ZNew );
             C2New := (1.0-COSH( SqrtZ )) / ZNew;
             C3New := (SINH( SqrtZ ) - SqrtZ) / ( SqrtZ*SqrtZ*SqrtZ );
           END
           ELSE
           BEGIN
             C2New := 0.5;
             C3New := 1.0/6.0;
           END;
       END;
   END;  { PROCEDURE FINDC2C3 }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE NEWTONE
|
|  This PROCEDURE solves Keplers equation when the Eccentric, paraboic, or
|    Hyperbolic anomalies are known. The Mean anomaly and true anomaly are
|    calculated.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Ecc         - Eccentricity                   0.0 to
|    E0          - Eccentric Anomaly              -2Pi to 2Pi rad
|
|  Outputs       :
|    M           - Mean Anomaly                   0.0 to 2Pi rad
|    Nu          - True Anomaly                   0.0 to 2Pi rad
|
|  Locals        :
|    Sinv        - Sine of Nu
|    Cosv        - Cosine of Nu
|
|  Coupling      :
|    ATAN2       - Arc tangent FUNCTION which also resloves quadrants
|    SINH        - Hyperbolic Sine
|    COSH        - Hyperbolic Cosine
|
|  References    :
|    Vallado       2007, 85, Alg 6
|
 ----------------------------------------------------------------------------- }

PROCEDURE NEWTONE            ( Ecc,E0                                : EXTENDED;
                               VAR M,Nu                              : EXTENDED );
   CONST
     Small      : EXTENDED =     0.00000001;       { Small value for tolerances }
   VAR
     Sinv,Cosv  : EXTENDED;
   BEGIN
     { -------------------------- Circular -------------------------- }
     IF ABS( Ecc ) < Small THEN
       BEGIN
         M := E0;
         Nu:= E0;
       END
       ELSE

         { ------------------------ Elliptical ---------------------- }
         IF Ecc < 0.999 THEN
           BEGIN
             M:= E0 - Ecc*SIN(E0);
             Sinv:= ( SQRT( 1.0-Ecc*Ecc ) * SIN(E0) ) /
                    ( 1.0-Ecc*COS(E0) );
             Cosv:= ( COS(E0)-Ecc ) / ( 1.0 - Ecc*COS(E0) );
             Nu  := ATAN2( Sinv,Cosv );
           END
           ELSE

             { ----------------------- Hyperbolic  ------------------ }
             IF Ecc > 1.0001 THEN
               BEGIN
                 M:= Ecc*SINH(E0) - E0;
                 Sinv:= ( SQRT( Ecc*Ecc-1.0 ) * SINH(E0) ) /
                        ( 1.0 - Ecc*COSH(E0) );
                 Cosv:= ( COSH(E0)-Ecc ) / ( 1.0 - Ecc*COSH(E0) );
                 Nu  := ATAN2( Sinv,Cosv );
               END
               ELSE

               { --------------------- Parabolic -------------------- }
               BEGIN
                 M:= E0 + (1.0/3.0)*E0*E0*E0;
                 Nu:= 2.0*ARCTAN(E0);
               END;
   END;  { PROCEDURE NEWTONE }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE NEWTONM
|
|  This PROCEDURE performs the Newton Rhapson iteration to find the
|    Eccentric Anomaly given the Mean anomaly.  The True Anomaly is also
|    calculated.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Ecc         - Eccentricity                   0.0 to
|    M           - Mean Anomaly                   -2Pi to 2Pi rad
|
|  Outputs       :
|    E0          - Eccentric Anomaly              0.0 to 2Pi rad
|    Nu          - True Anomaly                   0.0 to 2Pi rad
|
|  Locals        :
|    E1          - Eccentric Anomaly, next value  rad
|    Sinv        - Sine of Nu
|    Cosv        - Cosine of Nu
|    Ktr         - Index
|    R1r         - CUBIC roots - 1 to 3
|    R1i         - imaginary component
|    R2r         -
|    R2i         -
|    R3r         -
|    R3i         -
|    S           - Variables for parabolic solution
|    W           - Variables for parabolic solution
|
|  Coupling      :
|    ATAN2       - Arc tangent FUNCTION which also resloves quadrants
|    CUBIC       - Solves a CUBIC polynomial
|    POWER       - Raises a base Number to an arbitrary POWER
|    SINH        - Hyperbolic Sine
|    COSH        - Hyperbolic Cosine
|    SGN         - Returns the sign of an argument
|
|  References    :
|    Vallado       2007, 85, Alg 2, Ex 2-1
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE NEWTONM            ( Ecc,M                                 : EXTENDED;
                               VAR E0,Nu                             : EXTENDED );
   CONST
     NumIter    : INTEGER  =    50;
     Small      : EXTENDED =     0.00000001;       { Small value for tolerances }
   VAR
     Ktr : INTEGER;
     s, w, E1, Sinv, Cosv, R1r, R1i, R2r, R2i, R3r, R3i : EXTENDED;
   BEGIN
     { -------------------------- Hyperbolic  ----------------------- }
     IF (Ecc-1.0) > Small THEN
       BEGIN
         { ------------  Initial Guess ------------- }
         IF Ecc < 1.6 THEN
             IF ((M < 0.0) and (M > -Pi)) or (M > Pi) THEN
                 E0:= M - Ecc
               ELSE
                 E0:= M + Ecc
           ELSE
             IF (Ecc < 3.6) and (ABS(M) > Pi) THEN { just edges }
                 E0:= M - SGN(M)*Ecc
               ELSE
                 E0:= M/(Ecc-1.0); { best over 1.8 in middle }
         Ktr:= 1;
         E1 := E0 + ( ( M - Ecc*SINH(E0) + E0 ) / ( Ecc*COSH(E0) - 1.0 ) );
         WHILE ( ABS(E1-E0) > Small ) and ( Ktr <= NumIter ) DO
           BEGIN
             E0:= E1;
             E1:= E0 + ( ( M - Ecc*SINH(E0) + E0 ) / ( Ecc*COSH(E0) - 1.0 ) );
             INC( Ktr );
           END;
         { ---------  Find True Anomaly  ----------- }
         Sinv:= -( SQRT( Ecc*Ecc-1.0 ) * SINH(E1) ) / ( 1.0 - Ecc*COSH(E1) );
         Cosv:= ( COSH(E1) - Ecc ) / ( 1.0 - Ecc*COSH(E1) );
         Nu  := ATAN2( Sinv,Cosv );
       END
       ELSE
         { ---------------------- Parabolic ------------------------- }
         IF ABS( Ecc-1.0 ) < Small THEN
           BEGIN
             CUBIC( 1.0/3.0,0.0,1.0,-M, R1r,R1i,R2r,R2i,R3r,R3i );
             E0:= R1r;
{             WriteLn( outfile,'roots',r1r:11:7,r1i:11:7,r2r:11:7,r2i:11:7,r3r:11:7,r3i:11:7 );}
{                 S := 0.5 * (HalfPi - ARCTAN( 1.5*M ) );
                 W := ARCTAN( POWER( TAN( S ) ,1.0/3.0 ) );
                 E0:= 2.0*COT(2.0*W);}
             Ktr:= 1;
             Nu := 2.0 * ARCTAN(E0);
           END
           ELSE
             { --------------------- Elliptical --------------------- }
             IF Ecc > Small THEN
               BEGIN
                 { ------------  Initial Guess ------------- }
                 IF ((M < 0.0) and (M > -Pi)) or (M > Pi) THEN
                     E0:= M - Ecc
                   ELSE
                     E0:= M + Ecc;
                 Ktr:= 1;
                 E1 := E0 + ( M - E0 + Ecc*SIN(E0) ) / ( 1.0 - Ecc*COS(E0) );
                 WHILE ( ABS(E1-E0) > Small ) and ( Ktr <= NumIter ) DO
                   BEGIN
                     INC( Ktr );
                     E0:= E1;
                     E1:= E0 + ( M - E0 + Ecc*SIN(E0) ) /
                               ( 1.0 - Ecc*COS(E0) );
                   END;  { While }
                 { ---------  Find True Anomaly  ----------- }
                 Sinv:= ( SQRT( 1.0-Ecc*Ecc ) * SIN(E1) ) /
                        ( 1.0-Ecc*COS(E1) );
                 Cosv:= ( COS(E1)-Ecc ) / ( 1.0 - Ecc*COS(E1) );
                 Nu  := ATAN2( Sinv,Cosv );
               END
               ELSE
               { --------------------- Circular --------------------- }
               BEGIN
                 Ktr:= 0;   Nu:= M;    E0:= M;
               END;
     IF Ktr > NumIter THEN
         WriteLn( 'NewtonRhapson not converged in ',NumIter:3,' Iterations' );
   END;  { PROCEDURE NEWTONM }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE NEWTONNU
|
|  This PROCEDURE solves Keplers equation when the true anomaly is known.
|    The Mean and Eccentric, parabolic, or hyperbolic anomalies are also found.
|    The parabolic limit at 168ø is arbitrary.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Ecc         - Eccentricity                   0.0 to
|    Nu          - True Anomaly                   -2Pi to 2Pi rad
|
|  Outputs       :
|    E0          - Eccentric Anomaly              0.0 to 2Pi rad
|    M           - Mean Anomaly                   0.0 to 2Pi rad
|
|  Locals        :
|    SinE        - Sine of E
|    CosE        - Cosine of E
|    Ktr         - Index
|
|  Coupling      :
|    ATAN2       - Arc tangent FUNCTION which also resloves quadrants
|    ASINH     - Arc hyperbolic sine
|    SINH        - Hyperbolic Sine
|
|  References    :
|    Vallado       2007, 85, Alg 5
|
 ----------------------------------------------------------------------------- }

PROCEDURE NEWTONNU           ( Ecc,Nu                                : EXTENDED;
                               VAR E0,M                              : EXTENDED );
   VAR
     SinE, CosE : EXTENDED;
   BEGIN
     E0:= 999999.9;     M := 999999.9;
     { ---------------------------- Circular ------------------------ }
     IF ABS( Ecc ) < 0.000001 THEN
       BEGIN
         M := Nu;
         E0:= Nu;
       END
       ELSE
         { ----------------------- Elliptical ----------------------- }
         IF Ecc < 0.999 THEN
           BEGIN
             SinE:= ( SQRT( 1.0-Ecc*Ecc ) * SIN(Nu) ) / ( 1.0+Ecc*COS(Nu) );
             CosE:= ( Ecc + COS(Nu) ) / ( 1.0 + Ecc*COS(Nu) );
             E0  := ATAN2( SinE,CosE );
             M   := E0 - Ecc*SIN(E0);
           END
           ELSE
             { --------------------- Hyperbolic  -------------------- }
             IF Ecc > 1.0001 THEN
               BEGIN
                 IF ((Ecc > 1.0) and (ABS(Nu)+0.00001 < Pi-ACOS(1.0/Ecc)) ) THEN
                   BEGIN
                     SinE:= ( SQRT( Ecc*Ecc-1.0 ) * SIN(Nu) ) /
                            ( 1.0 + Ecc*COS(Nu) );
                     E0  := ASINH( SinE );
                     M   := Ecc*SINH(E0) - E0;
                   END;
               END
               ELSE
                 { ------------------ Parabolic --------------------- }
                 IF ABS(Nu) < 168.0/57.29578 THEN { TAN undef at 180 }
                   BEGIN
                     E0:= TAN( Nu*0.5 );
                     M := E0 + (E0*E0*E0)/3.0;
                   END;

     IF Ecc < 1.0 THEN  { Since B and H are areas }
       BEGIN
         MODFUNC( M,2.0*Pi );
         IF M < 0.0 THEN
             M:= M + 2.0*Pi;
         MODFUNC( E0,2.0*Pi );
       END;

   END;  { PROCEDURE NEWTONNU }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE KEPLER
|
|  This PROCEDURE solves Keplers problem for orbit determination and returns a
|    future Geocentric Equatorial (IJK) position and velocity vector.  The
|    solution uses Universal variables.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Ro          - IJK Position vector - initial  km
|    Vo          - IJK Velocity vector - initial  km / s
|    Dtsec       - Length of time to propagate    sec
|
|  OutPuts       :
|    R           - IJK Position vector            km
|    V           - IJK Velocity vector            km / s
|    Error       - Error flag                     'ok', ...
|
|  Locals        :
|    F           - f expression
|    G           - g expression
|    FDot        - f DOT expression
|    GDot        - g DOT expression
|    XOld        - Old Universal Variable X
|    XOldSqrd    - XOld squared
|    XNew        - New Universal Variable X
|    XNewSqrd    - XNew squared
|    ZNew        - New value of z
|    C2New       - C2(psi) FUNCTION
|    C3New       - C3(psi) FUNCTION
|    Dtsec       - change in time                 s
|    TimeNew     - New time                       s
|    RDotV       - Result of Ro DOT Vo
|    A           - Semi or axis                   km
|    Alpha       - Reciprocol  1/a
|    SME         - Specific Mech Energy           km2 / s2
|    Period      - Time period for satellite      s
|    S           - Variable for parabolic case
|    W           - Variable for parabolic case
|    H           - Angular momentum vector
|    Temp        - Temporary EXTENDED value
|    i           - index
|
|  Coupling      :
|    MAG         - Magnitude of a vector
|    DOT         - DOT product of two vectors
|    MODFUNC     - MOD function for Real variables
|    COT         - Cotangent FUNCTION
|    POWER       - Raise a Number to some POWER
|    SGN         - Sign of a Number +1 or -1
|    FINDC2C3    - Find C2 and C3 functions
|    CROSS       - CROSS product of two vectors
|
|  References    :
|    Vallado       2007, 101, Alg 8, Ex 2-4
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE KEPLER             ( Ro,Vo                                 : Vector;
                               Dtsec                                 : EXTENDED;
                               VAR R,V                               : Vector;
                               VAR Error                             : Str12 );
   CONST
     HalfPi     : EXTENDED =     0.5 * pi;  { 1.57079632679490; }
     TwoPi      : EXTENDED =     2.0 * pi;  { 6.28318530717959; }
     Small      : EXTENDED =     0.00000001;       { Small value for tolerances }
     Mu         : EXTENDED = 398600.4418;          { graviational parameter }
     { 2 less 0's }
     Infinite   : EXTENDED = 999999.9;             { Infinite value }
     NumIter    : INTEGER  =    35;
   VAR
     h     : Vector;
     Ktr,i : INTEGER;
     F, G, FDot, GDot, Rval, XOld, XOldSqrd, XNew, XNewSqrd, ZNew, p,
     C2New, C3New, DtNew, RDotV, A, Alpha, SME, Period, S, W, Temp : EXTENDED;
   BEGIN
     { ---------------------  Initialize values   ------------------- }
ktr := 0;
xOld:= 0.0;
ZNew:= 0.0;
     Error:= 'ok';

     IF ABS( Dtsec ) > Small THEN
       BEGIN
         MAG( Ro );
         MAG( Vo );
         RDotV:= DOT( Ro,Vo );

         { --------------  Find SME, Alpha, and A  ------------------ }
         SME:= ( Vo[4]*Vo[4]*0.5 ) - ( mu/Ro[4] );
         Alpha:= -SME*2.0/mu;

         IF ABS( SME ) > Small THEN   { circle, ellipse, hyperbola }
             A:= -mu / ( 2.0*SME )
           ELSE
             A:= Infinite;
         IF ABS( Alpha ) < Small THEN { Parabola }
             Alpha:= 0.0;

         { -------------   Setup initial guess for x  --------------- }
         { ---------------  Circle and Ellipse -------------- }
         IF Alpha >= Small THEN
           BEGIN
             Period:= TwoPi * SQRT( POWER( ABS(A),3.0 )/mu );
             { --- Next 2 lines needed for 2body multi-rev -- }
             IF ABS( Dtsec ) > ABS( Period ) THEN
                 Dtsec:= MODFUNC( Dtsec,Period );
             IF ABS(Alpha-1.0) > Small THEN
                  XOld := sqrt(mu) * Dtsec * Alpha
               ELSE
                  { ---- 1st guess can't be too close. ie a circle, r=a ---- }
                  XOld:= sqrt(mu) * Dtsec*Alpha*0.97;
           END
           ELSE
             { ------------------  Parabola  ---------------- }
             IF ABS( Alpha ) < Small THEN
               BEGIN
                 CROSS( ro,vo, h );
                 p:= h[4]*h[4]/mu;
                 S:= 0.5 * (HalfPi - ARCTAN( 3.0*SQRT( mu/(p*p*p) )* Dtsec ) );
                 W:= ARCTAN( POWER( TAN( S ) ,1.0/3.0 ) );
                 XOld := SQRT(p) * ( 2.0*COT(2.0*W) );
                 Alpha:= 0.0;
               END
               ELSE
                 { --------------  Hyperbola  --------------- }
                 BEGIN
                   Temp:= -2.0*mu*Dtsec /
                           ( A*( RDotV + SGN(Dtsec)*SQRT(-mu * A)*(1.0-Ro[4]*Alpha) ) );
                   XOld:= SGN( Dtsec ) * SQRT( -mu*A ) * LN( Temp );
                 END;
{}
         Ktr:= 1;
         REPEAT
             XOldSqrd := XOld*XOld;
             ZNew     := XOldSqrd * Alpha;

             { -------------- Find C2 and C3 functions -------------- }
             FINDC2C3( ZNew, C2New,C3New );

             { ------- Use a Newton iteration for New values -------- }
             DtNew:= XOldSqrd*XOld*C3New + RDotV*XOldSqrd*C2New/sqrt(mu) +
                       Ro[4]*XOld*( 1.0 - ZNew*C3New );
             Rval := XOldSqrd*C2New + RDotV*XOld*( 1.0 - ZNew*C3New )/sqrt(mu) +
                       Ro[4]*( 1.0 - ZNew*C2New );

             { ------------- Calculate New value for x -------------- }
             XNew := XOld + ( Dtsec*sqrt(mu)-DtNew ) / Rval;

             { -------------------------------------------------------------
             | Check IF the orbit is an ellipse and xNew > 2pi SQRT(a), the step
             |   size must be changed.  This is accomplished by multiplying Rval
             |   by 10.0.  NOTE !! 10.0 is arbitrary, but seems to produce good
             |   results.  The idea is to keep XNew from increasing too rapidily.
              -------------------------------------------------------------- }
(* including this doesn't work IF you don't MOD the Dtsec }
|             IF ( A > 0.0 ) and ( ABS(XNew)>TwoPi*SQRT(A) ) and ( SME < 0.0 ) THEN
|               BEGIN
|                 Dx:= ( Dtsec-DtNew ) / Rval; { *7.0  * 10.0 }
|                 XNew := XOld + Dx / 7.0;  { /(1.0 + Dx) }
|                 WriteLn( FileOut,'dx orig',dx:11:7,' d10 ',dx*0.1:11:7,
|                                  ' d7 ',dx/7.0:11:7,' dn ',dx/(1.0+dx):11:7 );
|{   Alternate method to test various values of change
|WriteLn('chgamt ',Ktr:2,' xn ',xNew:18:8,' rval ',RVal:18:8,' a ',a:18:8);
|XNew := XOld + ( Dtsec-DtNew ) / ( RVal*10 chgamt  );}
|               END;
*)
             IF Show IN ['Y','S'] THEN
                 WriteLn( FileOut,i:2,XOld:9:6,' ',DtNew:9:6,' ',Rval:10:6,' ',
                          XNew:10:6,C3New:10:6,C2New:10:6,zNew:10:6 );

               INC( Ktr );
             XOld := XNew;
         UNTIL ( ABS( DtNew/sqrt(mu)-Dtsec ) < Small ) or ( Ktr >= NumIter );

         IF Ktr >= NumIter THEN
           BEGIN
             Error:= 'KNotConv';
             WriteLn( ' Not converged in ',NumIter:2,' iterations ' );
             FOR i:= 1 to 4 DO
                 V[i]:= 0.0;
             R:= V;
           END
           ELSE
           { ---- Find position and velocity vectors at New time ---- }
           BEGIN
             XNewSqrd := XNew*XNew;
             F := 1.0 - ( XNewSqrd*C2New / Ro[4] );
             G := Dtsec - XNewSqrd*XNew*C3New/sqrt(mu);
             FOR i:= 1 to 3 DO
                 R[i]:= F*Ro[i] + G*Vo[i];
             MAG( R );
             GDot := 1.0 - ( XNewSqrd*C2New / R[4] );
             FDot := ( sqrt(mu) * XNew / ( Ro[4]*R[4] ) ) * ( ZNew*C3New - 1.0 );
             FOR i:= 1 to 3 DO
                 V[i]:= FDot*Ro[i] + GDot*Vo[i];
             MAG( V );
             Temp:= F*GDot - FDot*G;
             IF ABS(Temp-1.0) > 0.00001 THEN
               BEGIN
                 Error:= 'FandG';
                 WriteLn( ' Error in f and g in KEPLER ',Temp:14:10 );
               END;
             IF Show IN ['Y','S'] THEN
                 WriteLn( FileOut,'f g fgdot',f:11:7,g:11:7,FDot:11:7,GDot:11:7,
                                  (f*GDot-g*FDot):11:7 );
           END; { IF }
       END
       ELSE
       { ------------ Set vectors to incoming since 0 time ---------- }
       BEGIN
         r:= ro;
         v:= vo;
       END;
{ Write( FileOut,ZNew:12:5,Dtsec*13.44685108:11:4,xOld:12:5,Ktr:6 );}

   END;  { PROCEDURE KEPLER }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE FINDTOF
|
|  This PROCEDURE finds the time of flight given the initial position vectors,
|    Semi-parameter, and the sine and cosine values for the change in true
|    anomaly.  The result uses p-iteration theory to analytically find the result.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Ro          - Interceptor position vector    ER
|    R           - TARGET position vector         ER
|    p           - Semiparameter                  ER
|
|  Outputs       :
|    Tof         - Time for transfer              TU
|
|  Locals        :
|    SinDNu      - Sine of change in Nu           rad
|    CosDNu      - Cosine of change in Nu         rad
|    DeltaE      -
|    DeltaH      -
|    k           -
|    l           -
|    m           -
|    a           -
|    f           -
|    g           -
|    FDot        -
|    SinDeltaE   - Sine value
|    CosDeltaE   - Cosine value
|    RcrossR     - CROSS product of two positions
|
|  Coupling      :
|    CROSS       - CROSS product of two vectors
|    SINH        - Hyperbolic Sine
|    ACOSH     - Arc hyperbolic cosine
|    ATAN2       - Arc tangent FUNCTION which also resloves quadrants
|    POWER       - Raise a Number to some POWER
|
|  References    :
|    Vallado       2007, 134, Alg 11
|
 ----------------------------------------------------------------------------- }

PROCEDURE FINDTOF            ( Ro,R                                  : Vector;
                               p                                     : EXTENDED;
                               VAR Tof                               : EXTENDED );
   VAR
     RCrossR : Vector;
     CosDNu, SinDNu, Small, c , s, alpha, DeltaE, DeltaH, DNu,
     k, l, m, a, f, g, FDot, SinDeltaE, CosDeltaE : EXTENDED;
   BEGIN
     Small := 0.00001;
     CosDNu:= DOT(Ro,R)/(Ro[4]*R[4]);
     CROSS( Ro,R, RCrossR );
     SinDNu:= RCrossR[4]/(Ro[4]*R[4]);

     k:= Ro[4] * R[4]*( 1.0-CosDNu );
     l:= Ro[4] + R[4];
     m:= Ro[4] * R[4]*( 1.0+CosDNu );
     a:= (m*k*p) / ((2.0*m-l*l)*p*p + 2.0*k*l*p - k*k);

     { -------  Use F and G series to find Velocity Vectors  -------- }
     F := 1.0 - ( R[4]/p )*(1.0-CosDNu);
     G := Ro[4]*R[4]*SinDNu/SQRT(p);
     Alpha:= 1.0/a;
     IF alpha > Small THEN
       BEGIN
         { -------------------- Elliptical ------------------ }
         DNu  := ATAN2( SinDNu,CosDNu );
         FDot := SQRT(1.0/p) * TAN(DNu*0.5)*( ((1.0-CosDNu)/p)-
                         (1.0/Ro[4])-(1.0/R[4]) );
         COSDeltaE:= 1.0-(Ro[4]/a)*(1.0-f);
         SinDeltaE:= (-Ro[4]*R[4]*FDot)/SQRT(a);
         DeltaE   := ATAN2( SinDeltaE,CosDeltaE );
         Tof      := G + SQRT(a*a*a)*(DeltaE-SinDeltaE);
         IF Show = 'Y' THEN
           BEGIN
             WriteLn( FileOut,'f g fgdot',F:11:7,G:11:7,FDot:11:7 );
             WriteLn( FileOut,'DNu ',dNu*57.29577951:11:7,DeltaE*57.2957795:11:7 );
           END;
       END
       ELSE
         { -------------------- Hyperbolic ------------------ }
         IF alpha < -Small THEN
           BEGIN
             DeltaH := ACOSH( 1.0-(Ro[4]/a)*(1.0-F) );
             Tof    := G + SQRT(-a*a*a)*(SINH(DeltaH)-DeltaH);
           END
           ELSE
           BEGIN
             { ----------------- Parabolic ------------------ }
             DNu:= ATAN2( SinDNu,CosDNu );
             c  := SQRT( r[4]*r[4]+ro[4]*ro[4] - 2.0*r[4]*ro[4]*COS(DNu) );
             s  := (ro[4]+r[4]+c ) * 0.5;
             Tof:= ( 2.0/3.0 ) * SQRT(s*s*s*0.5) *(1.0 -  POWER((s-c)/s,1.5) );
             WriteLn( 'Inside parabolic ',Tof*13.44685:11:7 );
           END;

   END;  { PROCEDURE FINDTOF }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE ijk2ll
|
|  These PROCEDUREs convert a Geocentric Equatorial (IJK) position vector into
|    latitude and longitude.  Geodetic and Geocentric latitude are found.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Recef       - ECEF position vector            ER
|
|  OutPuts       :
|    Latgc       - Geocentric Latitude            -Pi to Pi rad
|    Latgd       - Geodetic Latitude              -Pi to Pi rad
|    Lon         - Longitude (WEST -)             -2Pi to 2Pi rad
|    Hellp       - Height above the ellipsoid     ER
|
|  Locals        :
|
|  Escobal:
|    Rc          - Range of SITE wrt earth center ER
|    Height      - Height above earth wrt SITE    ER
|    Alpha       - ANGLE from Iaxis to point, LST rad
|    OldDelta    - Previous value of DeltaLat     rad
|    DeltaLat    - Diff between Delta and
|                  Geocentric lat                 rad
|    Delta       - Declination ANGLE of R in IJK  rad
|    RSqrd       - Magnitude of r squared         ER2
|    SinTemp     - Sine of Temp                   rad
|
|  Almanac:
|    Temp        - Diff between Geocentric/
|                  Geodetic lat                   rad
|    SinTemp     - Sine of Temp                   rad
|    OldDelta    - Previous value of DeltaLat     rad
|    RtAsc       - Right ascension                rad
|    Decl        - Declination                    rad
|    c           -
|    i           - index
|
|  Borkowski:
|
|
|
|
|  Coupling      :
|    MAG         - Magnitude of a vector
|    ATAN2       - Arc Tangent which also resolves quadrant
|    ASIN      - Arc Sine of a value
|    GSTIME      - Greenwich SIDEREAL Time
|    gc2gd    - Converts between geocentric and geodetic latitude
|    SGN         - Sign of a number +1 or -1
|
|  References    :
|    Vallado       2007, 179, Alg 12 and Alg 13, Ex 3-3
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE ijk2llA       ( Recef                                    : Vector;
                             VAR Latgc,Latgd,Lon,Hellp             : EXTENDED );
   CONST
     TwoPi      : EXTENDED =     6.28318530717959;
     Small      : EXTENDED =     0.00000001;       { Small value for tolerances }
     EESqrd     : EXTENDED =     0.006694385000;   { Eccentricity of Earth Sqrd }
   VAR
     i : INTEGER;
     RtAsc, OldDelta, c, Decl, Temp, SinTemp : EXTENDED;
   BEGIN
     { ---------------------  Initialize values   ------------------- }
     MAG( Recef );

     { ------------------ Find Longitude value  --------------------- }
     Temp := SQRT( Recef[1]*Recef[1] + Recef[2]*Recef[2] );
     IF ABS( Temp ) < Small THEN
         RtAsc:= SGN(Recef[3])*Pi*0.5
       ELSE
         RtAsc:= ATAN2( Recef[2] / Temp , Recef[1] / Temp );
     Lon  := RtAsc;
     IF ABS(Lon) >= Pi THEN { Mod it ? }
         IF Lon < 0.0 THEN
             Lon:= TwoPi + Lon
           ELSE
             Lon:= Lon - TwoPi;
     Decl := ASIN( Recef[3] / Recef[4] );
     Latgd:= Decl;
     IF Show = 'Y' THEN
         WriteLn( RtAsc*57.29578:14:7,decl*57.29578:14:7,temp:11:7 );

     { -------------- Iterate to find Geodetic Latitude ------------- }
     i:= 1;
     REPEAT
         OldDelta:= Latgd;
         SinTemp := SIN( Latgd );
         c       := 1.0 / (SQRT( 1.0-EESqrd*SinTemp*SinTemp ));
         Latgd:= ARCTAN( (Recef[3]+c*EESqrd*SinTemp)/Temp );
         IF Show = 'Y' THEN
             WriteLn( 'A loops ',i:3,' gd ',Latgd*57.29578:16:10,c:11:7 );
         INC( i );
     UNTIL ( ABS( OldDelta - Latgd ) < Small ) or ( i >= 10 );

     Hellp   := (Temp/COS(Latgd)) - c;
     gc2gd( Latgc, FROM, Latgd );

     IF i >= 10 THEN
         WriteLn( 'ijk2ll did NOT converge in c2' );
   END;  { PROCEDURE ijk2llA }
{}
PROCEDURE ijk2llE       ( Recef                                     : Vector;
                               VAR Latgc,Latgd,Lon,Hellp             : EXTENDED );
   CONST
     TwoPi      : EXTENDED =     2.0 * pi;  { 6.28318530717959; }
     Small      : EXTENDED =     0.00000001;       { Small value for tolerances }
     EeSqrd     : EXTENDED =     0.006694385000;   { Eccentricity of Earth Sqrd }
   VAR
     rsite, RtAsc, OldDelta, DeltaLat, RSqrd,
     OneMinuse2, Decl, SinTemp,  Temp : EXTENDED;
     i : INTEGER;
   BEGIN
     { --------------------  Initialize values   -------------------- }
     MAG( Recef );
     OneMinuse2 := 1.0 - EeSqrd;

     { ----------------- Find Longitude value  ---------------------- }
     Temp := SQRT( Recef[1]*Recef[1] + Recef[2]*Recef[2] );
     IF ABS( Temp ) < Small THEN
         RtAsc:= SGN(Recef[3])*Pi*0.5
       ELSE
         RtAsc:= ATAN2( Recef[2] / Temp , Recef[1] / Temp );
     Lon  := RtAsc;

     IF ABS(Lon) >= Pi THEN { Mod it ? }
         IF Lon < 0.0 THEN
             Lon:= TwoPi + Lon
           ELSE
             Lon:= Lon - TwoPi;

     { --------------- Set up initial latitude value  --------------- }
     Decl    := ASIN( Recef[3] / Recef[4] );
     Latgc:= Decl;
     DeltaLat:= 100.0;
     RSqrd   := Recef[4]*Recef[4];

     { ----- Iterate to find Geocentric and Geodetic Latitude  ----- }
     i:= 1;
     REPEAT
         OldDelta := DeltaLat;
         rsite    := SQRT( OneMinuse2 / (1.0 - EeSqrd*SQR(COS(Latgc)) ) );
         Latgd := ARCTAN( TAN(Latgc) / OneMinuse2 );
         Temp     := Latgd-Latgc;
         SinTemp  := SIN( Temp );
         Hellp    := SQRT( RSqrd - rsite*rsite*SinTemp*SinTemp ) - rsite*COS(Temp);
         DeltaLat := ASIN( Hellp*SinTemp / Recef[4] );
         Latgc := Decl - DeltaLat;
         INC( i );
         IF Show = 'Y' THEN
             writeln( 'E loops gc gd ',Latgc*57.29578:12:6,Latgd*57.29578:16:10 );
     UNTIL ( ABS( OldDelta - DeltaLat ) < Small ) or ( i >= 10 );

     IF i >= 10 THEN
         WriteLn( 'ijk2ll did NOT converge ' );
   END;  { Procedure ijk2llE }
{}
PROCEDURE ijk2llB       ( Recef                                     : Vector;
                               VAR Latgc,Latgd,Lon,Hellp       : EXTENDED );
   CONST
     TwoPi      : EXTENDED =     2.0 * pi;  { 6.28318530717959; }
     Small      : EXTENDED =     0.00000001;       { Small value for tolerances }
   VAR
     a,b,RtAsc, sqrtp,third,e,f,p,q,d,nu,g,t,c, aTemp,  Temp: EXTENDED;
     i  : INTEGER;
   BEGIN
     { --------------------  Initialize values   -------------------- }
     MAG( Recef );

     { ----------------- Find Longitude value  ---------------------- }
     Temp := SQRT( Recef[1]*Recef[1] + Recef[2]*Recef[2] );
     IF ABS( Temp ) < Small THEN
         RtAsc:= SGN(Recef[3])*Pi*0.5
       ELSE
         RtAsc:= ATAN2( Recef[2] / Temp , Recef[1] / Temp );
     Lon  := RtAsc;
     IF ABS(Lon) >= Pi THEN { Mod it ? }
         IF Lon < 0.0 THEN
             Lon:= TwoPi + Lon
           ELSE
             Lon:= Lon - TwoPi;

     a:= 1.0;
     b:= SGN(Recef[3])*6356.75160056/6378.1363;
     { --------------- Set up initial latitude value  --------------- }
     aTemp:= 1.0/(a*Temp);
     e:= (b*Recef[3]-a*a+b*b)*atemp;
     f:= (b*Recef[3]+a*a-b*b)*atemp;
     third:= 1.0/3.0;
     p:= 4.0*Third*(e*f + 1.0 );
     q:= 2.0*(e*e - f*f);
     d:= p*p*p + q*q;

     IF d > 0.0 THEN
         nu:= POWER(SQRT(d)-q,third) - POWER(SQRT(d)+q,third)
       ELSE
       BEGIN
         SqrtP:= SQRT(-p);
         nu:= 2.0*SqrtP*COS( third*ACOS(q/(p*SqrtP)) );
       END;
     g:= 0.5*(SQRT(e*e + nu) + e);
     t:= SQRT(g*g + (f-nu*g)/(2.0*g-e)) - g;

     Latgd:= ARCTAN(a*(1.0-t*t)/(2.0*b*t));
     hellp:= (temp-a*t)*COS( Latgd) + (Recef[3]-b)*SIN(Latgd);

     gc2gd( Latgc, FROM, Latgd );
   END;  { Procedure ijk2llB }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE gc2gd
|
|  This PROCEDURE converts from Geodetic to Geocentric Latitude for positions
|    on the surface of the Earth.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Latgd       - Geodetic Latitude              -Pi to Pi rad
|    Direction   - Which set of vars to output    FROM  TOO
|
|  Outputs       :
|    Latgc       - Geocentric Latitude            -Pi to Pi rad
|
|  Locals        :
|    None.
|
|  Coupling      :
|    None.
|
|  References    :
|    Vallado       2007, 148, Eq 3-11
|
 ----------------------------------------------------------------------------- }

PROCEDURE gc2gd              ( VAR Latgc                             : EXTENDED;
                               Direction                             : Direct;
                               VAR Latgd                             : EXTENDED );
   CONST
     EESqrd     : EXTENDED =     0.006694385000;   { Eccentricity of Earth Sqrd }
   BEGIN
     IF Direction = FROM THEN
         Latgc:= ARCTAN( (1.0 - EESqrd)*TAN(Latgd) )
       ELSE
         Latgd:= ARCTAN( TAN(Latgc)/(1.0 - EESqrd) );
   END; { PROCEDURE gc2gd }

{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE SIGHT
|
|  This PROCEDURE takes the position vectors of two satellites and determines
|    IF there is line-of-SIGHT between the two satellites.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    R1          - Position vector of the 1st sat ER
|    R2          - Position vector of the 2nd sat ER
|    WhichKind   - Spherical or Ellipsoidal Earth 'S', 'E'*default
|
|  Outputs       :
|    LOS         - Line of SIGHT                  'YES','NO '
|
|  Locals        :
|    TR1         - Scaled R1 vector               ER
|    TR2         - Scaled R2 vector               ER
|    ADotB       - DOT product of a DOT b
|    TMin        - Minimum value of t from a to b
|    DistSqrd    - MIN Distance squared to Earth  ER
|    ASqrd       - Magnitude of A squared
|    BSqrd       - Magnitude of B squared
|
|  Coupling:
|    DOT         - DOT product of two vectors
|
|  References    :
|    Vallado       2007, 310, Alg 35, Ex 5-3
|
 ----------------------------------------------------------------------------- }

PROCEDURE SIGHT              ( R1,R2                                 : Vector;
                               WhichKind                             : CHAR;
                               VAR LOS                               : Str3      );
   CONST
     EESqrd     : EXTENDED =     0.006694385000;   { Eccentricity of Earth Sqrd }
   VAR
     TR1, TR2 : Vector;
     ADotB, TMin, DistSqrd, ASqrd, BSqrd, Temp : EXTENDED;
   BEGIN
     TR1:= R1;   TR2:= R2;
     { --------- Scale z component -------- }
     IF UPCASE(WhichKind) = 'E' THEN
         Temp:= 1.0/SQRT(1.0-EESqrd)
       ELSE
         Temp:= 1.0;
     TR1[3]:= TR1[3]*Temp;
     TR2[3]:= TR2[3]*Temp;

     BSqrd:= TR2[4]*TR2[4];
     ASqrd:= TR1[4]*TR1[4];
     ADotB:= DOT( TR1,TR2 );
     { ----------- Find TMin -------------- }
     DistSqrd:= 0.0;
     IF ABS(ASqrd + BSqrd - 2.0*ADotB) < 0.0001 THEN
         TMin:= 0.0
       ELSE
         TMin := ( ASqrd - ADotB ) / ( ASqrd + BSqrd - 2.0*ADotB );
     { ------------ Check LOS ------------- }
     IF (TMin < 0.0) or (TMin > 1.0) THEN
         LOS:= 'YES'
       ELSE
       BEGIN
         DistSqrd:= (1.0-TMin)*ASqrd + ADotB*TMin;
         IF DistSqrd > 1.0 THEN
             LOS:= 'YES'
           ELSE
             LOS:= 'NO ';
       END;

     IF Show = 'Y' THEN
         WriteLn( 'Denom ',(ASqrd + BSqrd - 2.0*ADotB),'TMin  ',TMin:11:7,'DistS ',DistSqrd:11:7 );
   END;  { PROCEDURE SIGHT }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE SUN
|
|  This PROCEDURE calculates the Geocentric Equatorial position vector for
|    the SUN given the Julian Date.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    JD          - Julian Date                    days from 4713 BC
|
|  Outputs       :
|    RSun        - IJK Position vector of the SUN AU
|    RtAsc       - Right Ascension                rad
|    Decl        - Declination                    rad
|
|  Locals        :
|    MeanLong    - Mean Longitude
|    MeanAnomaly - Mean anomaly
|    EclpLong    - Ecliptic Longitude
|    Obliquity   - Mean Obliquity of the Ecliptic
|    TUT1        - Julian Centuries of UT1 from
|                  Jan 1, 2000 12h
|    TTDB        - Julian Centuries of TDB from
|                  Jan 1, 2000 12h
|    Hr          - Hours                          0 .. 24
|    MIN         - Minutes                        0 .. 59
|    SEC         - Seconds                        0.0 .. 59.99
|    Temp        - Temporary variable
|    deg         - Degrees
|
|  Coupling      :
|    MODFUNC     - MOD FUNCTION for REAL variables
|    ASIN      - Arc sine FUNCTION
|
|  References    :
|    Vallado       2007, 281, Alg 29, Ex 5-1
|
 ----------------------------------------------------------------------------- }

PROCEDURE SUN                ( JD                                    : EXTENDED;
                               VAR RSun                              : Vector;
                               VAR RtAsc,Decl                        : EXTENDED );
   CONST
     TwoPi      : EXTENDED =     2.0 * pi;  { 6.28318530717959; }
     Deg2Rad    : EXTENDED =     pi / 180.0;  { 0.01745329251994; }
   VAR
     hr, MIN, deg : INTEGER;
     SEC, temp, MeanLong, MeanAnomaly, EclpLong, Obliquity, TUT1, TTDB : EXTENDED;
   BEGIN
     { --------------------  Initialize values   -------------------- }
     TUT1:= ( JD - 2451545.0 )/ 36525.0;

     MeanLong:= 280.4606184 + 36000.77005361*TUT1;
     MeanLong:= MODFUNC( MeanLong,360.0 ); {deg}

     TTDB:= TUT1;
     MeanAnomaly:= 357.5277233 + 35999.05034*TTDB;
     MeanAnomaly:= MODFUNC( MeanAnomaly*Deg2Rad,TwoPi ); {rad}
     IF MeanAnomaly < 0.0 THEN
         MeanAnomaly:= TwoPi + MeanAnomaly;

     EclpLong:= MeanLong + 1.914666471*SIN(MeanAnomaly)
                + 0.019994643*SIN(2.0*MeanAnomaly);{deg}

     Obliquity:= 23.439291 - 0.0130042*TTDB; {deg}

     MeanLong := MeanLong*Deg2Rad;
     IF MeanLong < 0.0 THEN
         MeanLong:= TwoPi + MeanLong;
     EclpLong := EclpLong *Deg2Rad;
     Obliquity:= Obliquity *Deg2Rad;

     { --------- Find magnitude of SUN vector, THEN components ------ }
     RSun[4]:= 1.000140612 - 0.016708617*COS( MeanAnomaly )
                           - 0.000139589*COS( 2.0*MeanAnomaly );   { in AU's }

     RSun[1]:= RSun[4]*COS( EclpLong );
     RSun[2]:= RSun[4]*COS(Obliquity)*SIN(EclpLong);
     RSun[3]:= RSun[4]*SIN(Obliquity)*SIN(EclpLong);

     RtAsc:= ARCTAN( COS(Obliquity)*TAN(EclpLong) );
     { ---- Check that RtAsc is in the same quadrant as EclpLong ---- }
     IF EclpLong < 0.0 THEN
         EclpLong:= EclpLong + TwoPi;   { make sure it's in 0 to 2pi range }
     IF ABS( EclpLong-RtAsc ) > Pi*0.5 THEN
         RtAsc:= RtAsc + 0.5*Pi*ROUND( (EclpLong-RtAsc)/(0.5*Pi) );
     Decl := ASIN( SIN(Obliquity)*SIN(EclpLong) );

     IF Show = 'Y' THEN
       BEGIN
         WriteLn('SUN Test Case ' );
         WriteLn( 'TUT1     ',TUT1:18:12 );
         WriteLn( 'Mean Lon ',MeanLong/Deg2Rad:15:9 );
         WriteLn( 'M        ',MeanAnomaly/Deg2Rad:15:9 );
         DMS_RAD( Deg,MIN,SEC,FROM, EclpLong );
         WriteLn( 'Ecl Lon  ',EclpLong/Deg2Rad:15:9,Deg:3,MIN:3,SEC:7:3 );
         DMS_RAD( Deg,MIN,SEC,FROM, Obliquity );
         WriteLn( 'Obliquit ',Obliquity/Deg2Rad:15:9,Deg:3,MIN:3,SEC:7:3 );
         HMS_RAD( Hr,MIN,SEC, FROM, RtAsc );
         WriteLn( 'RtAsc    ',RtAsc/Deg2Rad:15:9,Hr:3,MIN:3,SEC:7:3,'HMS' );
         DMS_RAD( Deg,MIN,SEC, FROM, Decl );
         WriteLn( 'decl     ',Decl/Deg2Rad:15:9,Deg:3,MIN:3,SEC:7:3 );
         WriteLn( RSun[1]:14:7,RSun[2]:14:7,RSun[3]:14:7,RSun[4]:18:7,' AU' );
         Temp:= 149597870.0;
         WriteLn( RSun[1]*temp:18:6,RSun[2]*Temp:18:6,RSun[3]*Temp:18:6,RSun[4]*Temp:18:6,' km' );
         WriteLn( 0.9772766*temp:18:6,0.1922635*Temp:18:6,0.0833613*Temp:18:6,' km' );
       END;
   END;  { PROCEDURE SUN }
{}
{
|    Vallado       2007, 311, Eq 5-9
}
PROCEDURE SunIll             ( JD, Lat, Lon                          : EXTENDED;
                               VAR SunIll,SunAz,SunEl                : EXTENDED );
   CONST
     AUER       : Extended = 149597870.0/6378.135;
     TwoPi      : EXTENDED =     2.0 * pi;  { 6.28318530717959; }
     Rad        : Extended =   180.0 / pi;  { 57.29577951308230; }
     Deg2Rad    : EXTENDED =     pi / 180.0;  { 0.01745329251994; }
   VAR
     RSun, RV, RhoSat : vector;
     lst,gst, x,g, LHA, sinv,cosv,
     l0,l1,l2,l3, sRtAsc, sdecl : Extended;
   BEGIN
     SUN( JD,  RSun,sRtAsc,sDecl ); { AU's needed for Sun ill }

     LSTIME( Lon,JD,  LST,GST );

     LHA := LST - sRtAsc;

     SunEl  := ASIN( SIN(sDecl)*SIN(Lat) + COS(sDecl)*COS(Lat)*COS(LHA) );

     Sinv:= -SIN(LHA)*COS(sDecl)*COS(Lat)/(COS(SunEl)*COS(Lat));
     Cosv:= ( SIN(sDecl)-SIN(SunEl)*SIN(Lat) )/(COS(SunEl)*COS(Lat));
     SunAz  := ATAN2( Sinv,Cosv );

     SunEl:= SunEl*rad; {deg}

     IF SunEl > -18.01 THEN { only do when Sun above astron horizon }
       BEGIN
         x:= SunEl/90.0;  { fractional part using deg }
         g:= 1.0;

         IF SunEl >= 20 THEN
           BEGIN
             l0:=  3.74;  l1:=  3.97;
             l2:= -4.07;  l3:=  1.47;
           END
           ELSE
             IF (SunEl >= 5.0) and (SunEl < 20.0) THEN
               BEGIN
                 l0:=   3.05;   l1:=  13.28;
                 l2:= -45.98;   l3:=  64.33;
               END
               ELSE
                 IF (SunEl >= -0.8) and (SunEl < 5.0) THEN
                   BEGIN
                     l0:=    2.88;   l1:=   22.26;
                     l2:= -207.64;   l3:= 1034.30;
                   END
                   ELSE
                     IF (SunEl >= -5.0) and (SunEl < -0.8) THEN
                       BEGIN
                         l0:=    2.88;   l1:=   21.81;
                         l2:= -258.11;   l3:= -858.36;
                       END
                       ELSE
                         IF (SunEl >= -12.0) and (SunEl < -5.0) THEN
                           BEGIN
                             l0:=    2.70;   l1:=   12.17;
                             l2:= -431.69;   l3:=-1899.83;
                           END
                           ELSE
                             IF (SunEl >= -18.0) and (SunEl < -12.0) THEN
                               BEGIN
                                 l0:=   13.84;   l1:=  262.72;
                                 l2:= 1447.42;   l3:= 2797.93;
                               END
                               ELSE
                               BEGIN
                                 l0:= 0.0;    l1:= 0.0;
                                 l2:= 0.0;    l3:= 0.0;
                               END;

         l1:= l0 + l1*x + l2*x*x + l3*x*x*x; { deg finds ill of full Sun with SunEl }
         SunIll:= POWER( 10.0,( l1 ) );
         IF (SunIll < -1E+36) or (SunIll > 999.999) THEN
             SunIll:= 0.0;
       END
       ELSE
         SunIll:= 0.0;

   END;  { Procedure SunIllumination }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE MOON
|
|  This PROCEDURE calculates the Geocentric Equatorial (IJK) position vector
|    for the MOON given the Julian Date.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    JD          - Julian Date                    days from 4713 BC
|
|  Outputs       :
|    RMoon       - IJK Position vector of MOON    ER
|    RtAsc       - Right Ascension                rad
|    Decl        - Declination                    rad
|
|  Locals        :
|    EclpLong    - Ecliptic Longitude
|    EclpLat     - Eclpitic Latitude
|    HzParal     - Horizontal Parallax
|    l           - Geocentric Direction Cosines
|    m           -             "     "
|    n           -             "     "
|    TTDB        - Julian Centuries of TDB from
|                  Jan 1, 2000 12h
|    Hr          - Hours                          0 .. 24
|    MIN         - Minutes                        0 .. 59
|    SEC         - Seconds                        0.0 .. 59.99
|    deg         - Degrees
|
|  Coupling      :
|    MODFUNC       MOD FUNCTION for REAL variables
|    ASIN        Arc Sine FUNCTION
|    ATAN2         Arc Tangent formula which resolves quadrants
|
|  References    :
|    Vallado       2007, 290, Alg 31, Ex 5-3
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE MOON               ( JD                                    : EXTENDED;
                               VAR RMoon                             : Vector;
                               VAR RtAsc,Decl                        : EXTENDED );
   CONST
     TwoPi      : EXTENDED =     2.0 * pi;  { 6.28318530717959; }
     Deg2Rad    : EXTENDED =     pi /180.0;  { 0.01745329251994; }
   VAR
     Hr, MIN, Deg : INTEGER;
     SEC, TTDB, l, m, n, Temp, Obliquity,
     EclpLong, EclpLat, HzParal : EXTENDED;
   BEGIN
     { --------------------  Initialize values   -------------------- }
     TTDB := ( JD - 2451545.0 ) / 36525.0;

     EclpLong:= 218.32 + 481267.883*TTDB
                + 6.29*SIN( (134.9+477198.85*TTDB)*Deg2Rad )
                - 1.27*SIN( (259.2-413335.38*TTDB)*Deg2Rad )
                + 0.66*SIN( (235.7+890534.23*TTDB)*Deg2Rad )
                + 0.21*SIN( (269.9+954397.70*TTDB)*Deg2Rad )
                - 0.19*SIN( (357.5+ 35999.05*TTDB)*Deg2Rad )
                - 0.11*SIN( (186.6+966404.05*TTDB)*Deg2Rad );     { Deg }

     EclpLat :=   5.13*SIN( ( 93.3+483202.03*TTDB)*Deg2Rad )
                + 0.28*SIN( (228.2+960400.87*TTDB)*Deg2Rad )
                - 0.28*SIN( (318.3+  6003.18*TTDB)*Deg2Rad )
                - 0.17*SIN( (217.6-407332.20*TTDB)*Deg2Rad );     { Deg }

     HzParal :=   0.9508 + 0.0518*COS( (134.9+477198.85*TTDB)*Deg2Rad )
                + 0.0095*COS( (259.2-413335.38*TTDB)*Deg2Rad )
                + 0.0078*COS( (235.7+890534.23*TTDB)*Deg2Rad )
                + 0.0028*COS( (269.9+954397.70*TTDB)*Deg2Rad );   { Deg }

     EclpLong := MODFUNC( EclpLong*Deg2Rad, TwoPi );
     EclpLat  := MODFUNC( EclpLat*Deg2Rad, TwoPi );
     HzParal  := MODFUNC( HzParal*Deg2Rad, TwoPi );

     Obliquity:= 23.439291 - 0.0130042*TTDB; {deg}
     Obliquity:= Obliquity *Deg2Rad;

     { ------------- Find the geocentric direction cosines ---------- }
     l:= COS( EclpLat ) * COS( EclpLong );
     m:= COS(Obliquity)*COS(EclpLat)*SIN(EclpLong)
         - SIN(Obliquity)*SIN(EclpLat);
     n:= SIN(Obliquity)*COS(EclpLat)*SIN(EclpLong)
         + COS(Obliquity)*SIN(EclpLat);

     { -------------- Calculate MOON position vector ---------------- }
     RMoon[4]:= 1.0/SIN( HzParal );
     RMoon[1]:= RMoon[4]*l;
     RMoon[2]:= RMoon[4]*m;
     RMoon[3]:= RMoon[4]*n;

     { --------------- Find Rt Ascension and Declination ------------ }
     RtAsc:= ATAN2( m,l );
     Decl := ASIN( n );

     IF Show = 'Y' THEN
       BEGIN
         WriteLn('MOON Test Case ' );
         WriteLn( 'TTDB     ',TTDB:18:12 );
         DMS_RAD( Deg,MIN,SEC,FROM, EclpLong );
         WriteLn( 'Ecl Lon  ',EclpLong/Deg2Rad:15:9,Deg:3,MIN:3,SEC:7:3 );
         DMS_RAD( Deg,MIN,SEC,FROM, EclpLat );
         WriteLn( 'Ecl Lat  ',EclpLat/Deg2Rad:15:9,Deg:3,MIN:3,SEC:7:3 );
         WriteLn( 'Parallax ',HzParal/Deg2Rad:15:9 );
         DMS_RAD( Deg,MIN,SEC,FROM, Obliquity );
         WriteLn( 'Obliquit ',Obliquity/Deg2Rad:15:9,Deg:3,MIN:3,SEC:7:3 );
         HMS_RAD( Hr,MIN,SEC, FROM, RtAsc );
         WriteLn( 'RtAsc    ',RtAsc/Deg2Rad:15:9,Hr:3,MIN:3,SEC:7:3,'HMS' );
         DMS_RAD( Deg,MIN,SEC, FROM, Decl );
         WriteLn( 'decl     ',Decl/Deg2Rad:15:9,Deg:3,MIN:3,SEC:7:3 );
         WriteLn( RMoon[1]:14:7,RMoon[2]:14:7,RMoon[3]:14:7,RMoon[4]:18:7,' ER' );
         Temp:= 6378.1363;
         WriteLn( RMoon[1]*temp:18:6,RMoon[2]*Temp:18:6,RMoon[3]*Temp:18:6,RMoon[4]*Temp:18:6,' km' );
       END;
   END;  { PROCEDURE MOON }
{}
{
|    Vallado       2007, 311, Eq 5-9
}
PROCEDURE MoonIll            ( MoonEl,f                              : EXTENDED;
                               VAR MoonIll                           : Extended );
   VAR
     x,g,l0,l1,l2,l3,HzParal : Extended;
   BEGIN
     x:= MoonEl/90.0;  { fractional part using deg }
     g:= 1.0;

     IF MoonEl >= 20 THEN
       BEGIN
         l0:= -1.95;  l1:=  4.06;
         l2:= -4.24;  l3:=  1.56;
       END
       ELSE
         IF (MoonEl >= 5.0) and (MoonEl < 20.0) THEN
           BEGIN
             l0:=  -2.58;   l1:=  12.58;
             l2:= -42.58;   l3:=  59.06;
           END
           ELSE
             IF (MoonEl > -0.8) and (MoonEl < 5.0) THEN
               BEGIN
                 l0:=   -2.79;   l1:=   24.27;
                 l2:= -252.95;   l3:= 1321.29;
               END
               ELSE
               BEGIN
                 l0:= 0.0;    l1:= 0.0;
                 l2:= 0.0;    l3:= 0.0;
                 f:= 0.0; g:= 0.0;
               END;

     l1:= l0 + l1*x + l2*x*x + l3*x*x*x; { deg finds ill of full moon with MoonEl }
     l2:= (-0.00868*f - 2.2E-9*f*f*f*f); { deg finds correct for phase        }
(*
     HzParal :=   0.9508 + 0.0518*COS( (134.9+477198.85*TTDB)*Deg2Rad )
                + 0.0095*COS( (259.2-413335.38*TTDB)*Deg2Rad )
                + 0.0078*COS( (235.7+890534.23*TTDB)*Deg2Rad )
                + 0.0028*COS( (269.9+954397.70*TTDB)*Deg2Rad );   { Deg }
     HzParal  := MODFUNC( HzParal*Deg2Rad, TwoPi );
     l3:= (2.0* POWER(10.0,(HzParal*rad / 0.951))*g ); { use g to eliminate neg el passes }
*)
     MoonIll:= POWER( 10.0,( l1 + l2 {+ l3} ) );
     IF (MoonIll < -1E+36) or (MoonIll > 0.999) THEN
         MoonIll:= 0.0;
   END;  { Procedure FindMoonIll }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE LIGHT
|
|  This PROCEDURE determines IF a spacecraft is sunlit or in the dark at a
|    particular time.  An oblate Earth and cylindrical shadow is assumed.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    R           - Position vector of sat         ER
|    JD          - Julian Date at desired time    Days from 4713 BC
|    WhichKind   - Spherical or Ellipsoidal Earth 'S', 'E'*default
|
|  OutPuts       :
|    Vis         - Visibility Flag                'YES','NO '
|
|  Locals        :
|    RtAsc       - Suns Right ascension           rad
|    Decl        - Suns Declination               rad
|    RSun        - SUN vector                     AU
|    AUER        - Conversion from AU to ER
|
|  Coupling      :
|    SUN         - Position vector of SUN
|    LNCOM1      - Multiple a vector by a constant
|    SIGHT       - Does Line-of-SIGHT exist beteen vectors
|
|  References    :
|    Vallado       2007, 310, Alg 35, Ex 5-6
|
 ----------------------------------------------------------------------------- }

PROCEDURE LIGHT              ( R                                     : Vector;
                               JD                                    : EXTENDED;
                               WhichKind                             : CHAR;
                               VAR LIT                               : Str3       );
   VAR
     RSun              : Vector;
     AUER, RtAsc, Decl : EXTENDED;
   BEGIN
     AUER:= 149597870.0/6378.1363;

     SUN( JD,RSun,RtAsc,Decl );
     LNCOM1( AUER,RSun, RSun );

     { ------------- Is the satellite in the shadow? ---------------- }
     SIGHT( RSun,R,WhichKind,  Lit );

   END; { PROCEDURE LIGHT }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE CHECKHITEARTH
|
|  This PROCEDURE checks to see IF the trajectory hits the earth during an orbital
|    transfer.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    RInt        - Initial Position vector of Int ER
|    V1t         - Initial Velocity vector of trnsER/TU
|    RTgt        - Initial Position vector of Tgt ER
|    V2t         - Final Velocity vector of trns  ER/TU
|
|  Outputs       :
|    HitEarth    - Is Earth was impacted          'Y' 'N'
|
|  Locals        :
|    SME         - Specific mechanical energy
|    rp          - Radius of Perigee              ER
|    TransA      - Semi-or axis of transfer       ER
|    TransE      - Eccentricity of transfer
|    TransP      - Semi-paramater of transfer     ER
|    HBar        - Angular momentum vector of
|                  transfer orbit
|
|  Coupling      :
|    DOT         - DOT product of vectors
|    MAG         - Magnitude of a vector
|    CROSS       - CROSS product of vectors
|
|  References    :
|    Vallado       2007, 500, Alg 60
|
  ---------------------------------------------------------------------------- }

PROCEDURE CHECKHITEARTH      ( Rint,V1t,Rtgt,V2t                     : Vector;
                               VAR HitEarth                          : CHAR );
   VAR
     HBar : Vector;
     SME, rp, TransP, TransA, TransE : EXTENDED;
   BEGIN
     HitEarth:= 'N';

     { ----------- Find IF trajectory intersects Earth -------------- }
     IF (DOT(Rint,V1t) < 0.0) and (DOT(RTgt,V2t) > 0.0) THEN
       BEGIN

        { ----------------  Find H N and E vectors   ---------------- }
         CROSS( RInt,V1t,HBar );
         MAG( HBar );

         IF HBar[4] > 0.00001 THEN
           BEGIN
           { ----------  Find a e and semi-Latus rectum   ----------- }
             SME    := V1t[4]*V1t[4]*0.5 - ( 1.0/RInt[4] );
             TransP := HBar[4]*HBar[4];
             TransE := 1.0;
             IF ABS( SME ) > 0.00001 THEN
               BEGIN
                 TransA:= -1.0 / (2.0*SME);
                 TransE:= SQRT( (TransA - TransP)/TransA );
                 rp:= TransA*(1.0-TransE);
               END
               ELSE
                 rp:= TransP*0.5;  { Parabola }

             IF ABS( rp ) < 1.0 THEN
                 HitEarth:= 'Y';
           END
           ELSE
             Write( 'The orbit does not exist ');
       END;

   END;  { PROCEDURE CHECKHITEARTH }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE SATFOV
|
|  This PROCEDURE finds parameters reLating to a satellite's FOV.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Incl        - Inclination                    rad
|    Az          - Azimuth                        rad
|    SLatgd      - Geodetic Latitude of sat       rad
|    SLon        - Longitude of sat               rad
|    SAlt        - Altitudeof satellite           ER
|    TFOV        - Total field of view            rad
|    EtaCtr      - Ctr where sensor looks         rad
|
|  Outputs       :
|    FovMax      - Maximum field of view          rad
|    TotalRng    -
|    RhoMax      -
|    RhoMin      -
|    TgtLat      -
|    TgtLon      -
|
|  Locals        :
|    r           -
|    etaHopriz   -
|    RhoHoriz    -
|    gamma       -
|    rho         -
|    FovMin      -
|    Lat         -
|    Lon         -
|    MaxLat      -
|    MinLKat     -
|    i           - Index
|
|  Coupling      :
|    ASIN      - Arc Sine FUNCTION
|    PATH        - Finds tgt location given initial location, range, and az
|
|  References    :
|    Vallado       2007, 845, Eq 11-8 to Eq 11-13, Ex 11-1
|
  ---------------------------------------------------------------------------- }
{}
PROCEDURE SATFOV             ( Incl,Az, SLatgd, SLon, SAlt, tFOV,
                               EtaCtr                                : EXTENDED;
                               VAR FovMax, TotalRng, RhoMax, RhoMin,
                               TgtLat, TgtLon                        : EXTENDED );
   CONST
     Rad2Deg    : EXTENDED =    180.0 / pi;  { 57.29577951308230; }
   VAR
     i : INTEGER;
     r, EtaHoriz, rhoHoriz, gammam,rhom,gamma, rho, FovMin, Lat, Lon, maxLat, minLat : EXTENDED;
   BEGIN
     { -------- Find satellite parameters and limiting cases -------- }
     r       := 1.0 + SAlt;
     EtaHoriz:= ASIN(1.0/r);
     RhoHoriz:= r*COS(EtaHoriz);

     { ----------------- Find Ground range ANGLE -------------------- }
     FovMax:= tFOV*0.5 + EtaCtr;
     GammaM:= Pi - ASIN( r*SIN(FovMax) );  { must use larger ANGLE }
     RhoM  := COS( GammaM ) + r*COS(FovMax);
     RhoMax:= ASIN( RhoM*SIN(FovMax) );

     { ----- Do minimum, IF the sensor looks off axis ----- }
     IF ABS(EtaCtr) > 0.00001 THEN
       BEGIN
         FovMin  := EtaCtr - tFOV*0.5;
         Gamma   := Pi - ASIN( r*SIN(FovMin) ); { use larger }
         Rho     := COS( gamma ) + r*COS(FovMin);
         RhoMin  := ASIN( Rho*SIN(FovMin) );
         TotalRng:= RhoMax - RhoMin;
       END
       ELSE
       { ----------------- Nadir pointing ----------------- }
       BEGIN
         Gamma   := 0.0;
         Rho     := 0.0;
         FovMin  := 0.0;
         RhoMin  := 0.0;
         TotalRng:= 2.0*RhoMax; { equal sided }
       END;

     { --------------- Find location of center of FOV --------------- }
     IF ABS(EtaCtr) > 0.00001 THEN
         PATH( SLatgd,SLon,RhoMin + TotalRng*0.5,Az,   Lat,Lon )
       ELSE
       BEGIN
         Lat:= SLatgd;
         Lon:= SLon;
       END;

     { ------ Loop around the New circle with the sensor range ------ }
     FOR i:= 0 to 72 DO
       BEGIN
         Az:= i*5.0/Rad2Deg;
         PATH( Lat,Lon,TotalRng*0.5,Az,   TgtLat,TgtLon );
         IF i = 0 THEN
           BEGIN
             MaxLat:= TgtLat;
             WriteLn(  'MAX',Az*Rad2Deg:11:7,TgtLat*Rad2Deg:14:7,
                  TgtLon*Rad2Deg:14:7 );
           END;
         IF i = 36 THEN
           BEGIN
             MinLat:= TgtLat;
             WriteLn(  'MIN',Az*Rad2Deg:11:7,TgtLat*Rad2Deg:14:7,
                  TgtLon*Rad2Deg:14:7 );
           END;
         IF i < 5 THEN
             WriteLn(  Az*Rad2Deg:11:7,TgtLat*Rad2Deg:14:7,
                  TgtLon*Rad2Deg:14:7 );
       END;

     IF Show = 'Y' THEN
       BEGIN
         WriteLn( r:11:7,EtaHoriz*Rad2Deg:14:7,rhoHoriz:11:7,rhoHoriz*6378.1363:14:7 );
         WriteLn( FovMax*Rad2Deg:11:7,gammaM*Rad2Deg:14:7,rhoM:12:7,rhoM*6378.1363:14:7,
                  RhoMax*Rad2Deg:14:7,RhoMax*6378.1363:14:7 );
         WriteLn( FovMin*Rad2Deg:11:7,gamma*Rad2Deg:14:7,rho:12:7,rho*6378.1363:14:7,
                  RhoMin*Rad2Deg:14:7,RhoMin*6378.1363:14:7 );
         WriteLn( 'total rng ',totalrng*Rad2Deg:14:7,totalrng*6378.1363:14:7 );
         WriteLn( Lat*Rad2Deg:11:7,Lon*Rad2Deg:11:7 );
       END;

   END;  { PROCEDURE SATFOV }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE RNGAZ
|
|  This PROCEDURE calculates the Range and Azimuth between two specified
|    ground points on a spherical Earth.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    LLat        - Start Geocentric Latitude      -Pi/2 to  Pi/2 rad
|    LLon        - Start Longitude (WEST -)       0.0 to 2Pi rad
|    TLat        - END Geocentric Latitude        -Pi/2 to  Pi/2 rad
|    TLon        - END Longitude (WEST -)         0.0 to 2Pi rad
|    Tof         - Time of Flight IF ICBM, or 0.0 TU
|
|  OutPuts       :
|    Range       - Range between points           ER
|    Az          - Azimuth                        0.0 to 2Pi rad
|
|  Locals        :
|    None.
|
|  Coupling      :
|    ACOS      - Arc Cosine FUNCTION
|
|  References    :
|    Vallado       2007, 843, Eq 11-3, Eq 11-4, Eq 11-5
|
 ----------------------------------------------------------------------------- }

PROCEDURE RNGAZ              ( LLat,LLon,TLat,TLon,Tof               : EXTENDED;
                               VAR Range, Az                         : EXTENDED );
   CONST
     TwoPi      : EXTENDED =     2.0 * pi;  { 6.28318530717959; }
     Small      : EXTENDED =     0.00000001;          { Small value for tolerances }
     OmegaEarth : EXTENDED =     0.05883359221938136; { Earth Rot rad/TU }
   BEGIN
     Range:= ACOS( SIN(LLat)*SIN(TLat) +
                     COS(LLat)*COS(TLat)*COS(TLon-LLon + OmegaEarth*Tof) );

     { ------- Check IF the Range is 0 or half the earth  ----------- }
     IF ABS( SIN(Range)*COS(LLat) ) < Small THEN
         IF ABS( Range - Pi ) < Small THEN
             Az:= Pi
           ELSE
             Az:= 0.0
       ELSE
         Az:= ACOS( ( SIN(TLat) - COS(Range) * SIN(LLat)) /
                      ( SIN(Range) * COS(LLat)) );

     { ------- Check IF the Azimuth is grt than Pi ( 180deg ) ------- }
     IF SIN( TLon - LLon + OmegaEarth*Tof ) < 0.0 THEN
         Az:= TwoPi - Az;
   END;   { PROCEDURE RNGAZ }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE PATH
|
|  This PROCEDURE determines the END position for a given range and azimuth
|    from a given point.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    LLat        - Start Geocentric Latitude      -Pi/2 to  Pi/2 rad
|    LLon        - Start Longitude (WEST -)       0.0 to 2Pi rad
|    Range       - Range between points           ER
|    Az          - Azimuth                        0.0 to 2Pi rad
|
|  OutPuts       :
|    TLat        - END Geocentric Latitude        -Pi/2 to  Pi/2 rad
|    TLon        - END Longitude (WEST -)         0.0 to 2Pi rad
|
|  Locals        :
|    SinDeltaN   - Sine of Delta N                rad
|    CosDeltaN   - Cosine of Delta N              rad
|    DeltaN      - ANGLE bteween the two points   rad
|
|  Coupling      :
|    ASIN      - Arc sine FUNCTION
|    MODFUNC     - MOD FUNCTION for REAL variables
|    ATAN2       - Arc tangent FUNCTION which also resolves quadrants
|
|  References    :
|    Vallado       2007, 843, Eq 11-6, Eq 11-7
|
 ----------------------------------------------------------------------------- }

PROCEDURE PATH               ( LLat, LLon, Range, Az                 : EXTENDED;
                               VAR TLat, TLon                        : EXTENDED );
   CONST
     TwoPi      : EXTENDED =     2.0 * pi;  { 6.28318530717959; }
     Small      : EXTENDED =     0.00000001;       { Small value for tolerances }
   VAR
     SinDN, CosDN, DeltaN : EXTENDED;
   BEGIN
     Az:= MODFUNC( Az,TwoPi );
     IF LLon < 0.0 THEN
         LLon:= TwoPi + LLon;
     IF Range > TwoPi THEN
         Range:= MODFUNC( Range,TwoPi );

     { ------------------ Find Geocentric Latitude  ----------------- }
     TLat := ASIN( SIN(LLat)*COS(Range) + COS(LLat)*SIN(Range)*COS(Az) );

     { ----- Find Delta N, the ANGLE between the points ------------- }
     IF (ABS(COS(TLat)) > Small) and (ABS(COS(LLat)) > Small) THEN
       BEGIN
         SinDN := SIN(Az)*SIN(Range) / COS(TLat);
         CosDN := ( COS(Range)-SIN(TLat)*SIN(LLat) ) /
                  ( COS(TLat)*COS(LLat) );
         DeltaN:= ATAN2(SinDN,CosDN);
       END
       ELSE
       BEGIN
         { ------- Case where launch is within 3nm of a Pole -------- }
         IF ABS(COS(LLat)) <= Small THEN
             IF (Range > Pi) and (Range < TwoPi) THEN
                 DeltaN:= Az + Pi
               ELSE
                 DeltaN:= Az;
         { ------ Case where END point is within 3nm of a pole ------ }
         IF ABS( COS(TLat) ) <= Small THEN
             DeltaN:= 0.0;
       END;

     TLon:= LLon + DeltaN;
     IF ABS(TLon) > TwoPi THEN
         TLon:= MODFUNC( TLon,TwoPi );
     IF TLon < 0.0 THEN
         TLon:= TwoPi + TLon;
   END;  { PROCEDURE PATH }


BEGIN

END.  { Unit Ast2Body }
{}
