(*     ----------------------------------------------------------------      *)

                               UNIT ASTMANV;
                                                                           
(*  this file contains fundamental astrodynamic procedures and functions     
*  relating to orbit transfer calculations. ch 6 describes each of these    
*  routines.                                                                
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
*                           3rd printing baseline
*    changes :
*              21 jul 05  david vallado
*                           2nd printing baseline
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
       AstTime; { for show and fileout}

{}
     { ---------- Routines for Orbit Transfer calculations --------- }

PROCEDURE Hohmann            ( RInit,RFinal,eInit,eFinal,
                               NuInit,NuFinal                        : EXTENDED;
                               VAR Deltava,Deltavb,DtTU              : EXTENDED );

PROCEDURE BiElliptic         ( RInit,Rb,RFinal,eInit,eFinal,
                               NuInit,NuFinal                        : EXTENDED;
                               VAR Deltava,Deltavb,DeltaVc,DtTU      : EXTENDED );

PROCEDURE OneTangent         ( RInit,RFinal,eInit,eFinal,
                               NuInit,NuTran                         : EXTENDED;
                               VAR Deltava,Deltavb,DtTU              : EXTENDED );

PROCEDURE IOnlyChg           ( Deltai,VInit,fpa                      : EXTENDED;
                               VAR DeltaViOnly                       : EXTENDED );

PROCEDURE NodeOnlyChg        ( iInit,ecc,DeltaOmega,VInit,fpa,incl   : EXTENDED;
                               VAR iFinal,DeltaV                     : EXTENDED );

PROCEDURE IandNodeChg        ( iInit,DeltaOmega,Deltai,VInit,fpa     : EXTENDED;
                               VAR DeltaV,iFinal                     : EXTENDED );

PROCEDURE MinCombinedPlaneChg( RInit,RFinal,eInit,eFinal,NuInit,
                               NuFinal,iInit,iFinal                  : EXTENDED;
                               VAR Deltai,Deltai1,DeltaVa,DeltaVb,
                               DtTU                                  : EXTENDED );

PROCEDURE CombinedPlaneChg   ( RInit,RFinal,eInit,e2,eFinal,NuInit,
                               Nu2a,Nu2b,NuFinal,Deltai              : EXTENDED;
                               VAR Deltava,Deltavb,DtTU              : EXTENDED );

PROCEDURE Rendezvous         ( Rcs1,Rcs3,PhaseI,eInit,eFinal,NuInit,
                               NuFinal                               : EXTENDED;
                               kTgt,kInt                             : INTEGER;
                               VAR PhaseF,WaitTime,DeltaV            : EXTENDED );

PROCEDURE NonCoplanarRendz   ( PhaseNew,Deltai,Delta2Node,LonTrue,
                               RInt,RTgt                             : EXTENDED;
                               kTgt,kInt                             : INTEGER;
                               VAR TTrans,TPhase,DVPhase,
                               DVTrans1,DVTrans2                     : EXTENDED );

PROCEDURE HillsR             ( R,V                                   : Vector;
                               Alt,DtTU                              : EXTENDED;
                               VAR RInit,VInit                       : Vector   );

PROCEDURE HillsV             ( R                                     : Vector;
                               Alt,DtTU                              : EXTENDED;
                               VAR V                                 : Vector   );

PROCEDURE Cow2Hill           ( rtgt,vtgt,rint,vint                   : Vector;
                               VAR RHill,VHill                       : VECTOR   );

PROCEDURE Hill2Cow           ( rTgtijk,vTgtijk,RHill,VHill           : VECTOR;
                               VAR rIntijk,vIntijk                   : Vector   );

{ ------------------- Constants used in this Library ------------------
|    OmegaEarth    0.05883359980154919  Angular rotation of Earth (Rad/TU)
|    RadPerDay     6.30038809866574     Rads Earth rotates in 1 Solar Day
|    TUMin        13.44685115881        Minutes per Time Unit
|    TUDaySun     58.1324409            days per sun TU
|    EESqrd        0.00669437999013     Eccentricity of Earth's shape squared
|    Flat          0.003352810664747352 Flatenning of the Earth
|    TUDay         0.0093380913806      Solar days per Time Unit
 ----------------------------------------------------------------------- }

(*     ----------------------------------------------------------------      *)

                                IMPLEMENTATION

(*     ----------------------------------------------------------------      *)
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE HOHMANN
|
|  This PROCEDURE calculates the delta v's for a Hohmann transfer for either
|    circle to circle, or ellipse to ellipse.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    RInit       - Initial position magnitude     ER
|    RFinal      - Final position magnitude       ER
|    eInit       - Eccentricity of first orbit
|    eFinal      - Eccentricity of final orbit
|    NuInit      - True Anomaly of first orbit    0 or Pi rad
|    NuFinal     - True Anomaly of final orbit    0 or Pi rad
|
|  OutPuts       :
|    DeltaVa     - Change in velocity at point a  ER / TU
|    DeltaVb     - Change in velocity at point b  ER / TU
|    DtTU        - Time of Flight for the trans   TU
|
|  Locals        :
|    SME1        - Mech Energy of first orbit     ER2 / TU
|    SME2        - Mech Energy of transfer orbit  ER2 / TU
|    SME3        - Mech Energy of final orbit     ER2 / TU
|    VInit       - Velocity of first orbit at a   ER / TU
|    vTransa     - Velocity of trans orbit at a   ER / TU
|    vTransb     - Velocity of trans orbit at b   ER / TU
|    VFinal      - Velocity of final orbit at b   ER / TU
|    aInit       - Semimajor axis of first orbit  ER
|    aTrans      - Semimajor axis of Trans orbit  ER
|    aFinal      - Semimajor axis of final orbit  ER
|
|  Coupling      :
|    None.
|
|  References    :
|    Vallado       2007, 327, Alg 36, Ex 6-1
|
 ----------------------------------------------------------------------------- }

PROCEDURE Hohmann            ( RInit,RFinal,eInit,eFinal,
                               NuInit,NuFinal                        : EXTENDED;
                               VAR Deltava,Deltavb,DtTU               : EXTENDED );
   CONST
     Pi  : EXTENDED = 3.14159265358979;
   VAR
     VInit,VTrana,VTranb,VFinal, aInit,aTran,aFinal  : EXTENDED;
   BEGIN
     { --------------------  Initialize values   ------------------- }
     aInit  := (rInit*(1.0+eInit*COS(NuInit))) / (1.0 - eInit*eInit );
     aTran  := ( RInit + RFinal ) / 2.0;
     aFinal := (rFinal*(1.0+eFinal*COS(NuFinal))) / (1.0 - eFinal*eFinal );
     DeltaVa:= 0.0;
     DeltaVb:= 0.0;
     DtTU   := 0.0;

     IF ( eInit < 1.0 ) or ( eFinal < 1.0 ) THEN
       BEGIN
         { -----------------  Find Delta v at point a  -------------- }
         VInit  := SQRT( 2.0/rInit - (1.0/aInit) );
         VTrana := SQRT( 2.0/rInit - (1.0/aTran) );
         DeltaVa:= ABS( VTrana - VInit );

         { -----------------  Find Delta v at point b  -------------- }
         VFinal := SQRT( 2.0/rFinal - (1.0/aFinal) );
         VTranb := SQRT( 2.0/rFinal - (1.0/aTran) );
         DeltaVb:= ABS( VFinal - VTranb );

         { ----------------  Find Transfer Time of Flight  ---------- }
         DtTU:= Pi * SQRT( aTran*aTran*aTran );

         IF Show IN ['Y','S'] THEN
           BEGIN
             WriteLn( FileOut,'aInit  ',aInit:10:6,' ER' );
             WriteLn( FileOut,'aTran  ',aTran:10:6,' ER' );
             WriteLn( FileOut,'aFinal ',aFinal:10:6,' ER' );
             WriteLn( FileOut,'VInit  ', vInit:10:6,' VTrana ',VTrana:10:6,' DVa ',DeltaVa:10:6 );
             WriteLn( FileOut,'Vtranb ',vTranb:10:6,' VFinal  ', vFinal:10:6,' DVb ',DeltaVb:10:6 );
             WriteLn( FileOut,'Total DV  ',(DeltaVa+DeltaVb):10:6,(Deltava+Deltavb)*7.90536599:12:7,' ER/TU' );
             WriteLn( FileOut,'Total DtTU ',DtTU:11:6,DtTU*13.44685115881:13:6,' min',DtTU*13.44685115881/60:11:6,' hrs' );
           END;
       END;
   END;  { PROCEDURE Hohmann }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE BIELLIPTIC
|
|  This PROCEDURE calculates the delta v's for a Bi-elliptic transfer for either
|    circle to circle, or ellipse to ellipse.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    RInit       - Initial position magnitude     ER
|    R2          - Interim orbit magnitude        ER
|    RFinal      - Final position magnitude       ER
|    eInit       - Eccentricity of first orbit
|    eFinal      - Eccentricity of final orbit
|    NuInit      - True Anomaly of first orbit    0 or Pi rad
|    NuFinal     - True Anomaly of final orbit    0 or Pi rad, Opp of NuInit
|
|  OutPuts       :
|    DeltaVa     - Change in velocity at point a  ER / TU
|    DeltaVb     - Change in velocity at point b  ER / TU
|    DtTU        - Time of Flight for the trans   TU
|
|  Locals        :
|    SME1        - Mech Energy of first orbit     ER2 / TU
|    SME2        - Mech Energy of transfer orbit  ER2 / TU
|    SME3        - Mech Energy of final orbit     ER2 / TU
|    VInit       - Velocity of first orbit at a   ER / TU
|    vTransa     - Velocity of trans orbit at a   ER / TU
|    vTransb     - Velocity of trans orbit at b   ER / TU
|    VFinal      - Velocity of final orbit at b   ER / TU
|    aInit       - Semimajor axis of first orbit  ER
|    aTrans      - Semimajor axis of Trans orbit  ER
|    aFinal      - Semimajor axis of final orbit  ER
|
|  Coupling      :
|    None.
|
|  References    :
|    Vallado       2007, 327, Alg 37, Ex 6-2
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE BiElliptic         ( RInit,Rb,RFinal,eInit,eFinal,
                               NuInit,NuFinal                        : EXTENDED;
                               VAR Deltava,Deltavb,DeltaVc,DtTU       : EXTENDED );
   CONST
      Pi  : EXTENDED = 3.14159265358979;
   VAR
      VInit,VTran1a,VTran1b,VTran2b,VTran2c,VFinal, aInit,aTran1,aTran2,aFinal  : EXTENDED;
   BEGIN
     { --------------------  Initialize values   ------------------- }
     aInit  := (rInit*(1.0+eInit*COS(NuInit))) / (1.0 - eInit*eInit );
     aTran1 := (RInit + Rb) * 0.5;
     aTran2 := (Rb + RFinal) * 0.5;
     aFinal := (rFinal*(1.0+eFinal*COS(NuFinal))) / (1.0 - eFinal*eFinal );

     DeltaVa:= 0.0;
     DeltaVb:= 0.0;
     DeltaVc:= 0.0;
     DtTU   := 0.0;

     IF ( eInit < 1.0 ) and ( eFinal < 1.0 ) THEN
       BEGIN

     { -----------------  Find Delta v at point a  ----------------- }
         VInit  := SQRT( 2.0/rInit - (1.0/aInit) );
         VTran1a:= SQRT( 2.0/rInit - (1.0/aTran1) );
         DeltaVa:= ABS( VTran1a - VInit );

     { -----------------  Find Delta v at point b  ----------------- }
         VTran1b:= SQRT( 2.0/rb - (1.0/aTran1) );
         VTran2b:= SQRT( 2.0/rb - (1.0/aTran2) );
         DeltaVb:= ABS( VTran1b - VTran2b );

     { -----------------  Find Delta v at point c  ----------------- }
         VTran2c:= SQRT( 2.0/rFinal - (1.0/aTran2) );
         VFinal := SQRT( 2.0/rFinal - (1.0/aFinal) );
         DeltaVc:= ABS( VFinal - VTran2c );

     { ----------------  Find Transfer Time of Flight  ------------- }
         DtTU:= Pi * SQRT( aTran1*aTran1*aTran1 ) +
                Pi * SQRT( aTran2*aTran2*aTran2 );

         IF Show IN ['Y','S'] THEN
           BEGIN
             WriteLn( FileOut,'aInit  ',aInit:10:6,' ER' );
             WriteLn( FileOut,'aTran1 ',aTran1:10:6,' ER' );
             WriteLn( FileOut,'aTran2 ',aTran2:10:6,' ER' );
             WriteLn( FileOut,'aFinal ',aFinal:10:6,' ER' );
             WriteLn( FileOut,'VInit   ', vInit:10:6,  ' VTran1a ',VTran1a:10:6,' DVa ',DeltaVa:10:6 );
             WriteLn( FileOut,'VTran1b ', vTran1b:10:6,' VTran2b ',VTran2b:10:6,' DVb ',DeltaVb:10:6 );
             WriteLn( FileOut,'VTran2c ', vTran2c:10:6,' VFinal  ', VFinal:10:6,' DVc ',DeltaVc:10:6 );
             WriteLn( FileOut,'Total DV  ',(DeltaVa+DeltaVb+Deltavc):10:6,(Deltava+Deltavb+Deltavc)*7.90536599:12:7,
                              ' ER/TU' );
             WriteLn( FileOut,'Total DtTU ',DtTU:11:6,DtTU*13.44685115881:13:6,' min',DtTU*13.44685115881/60:11:6,' hrs' );
           END;
       END;
   END;  { PROCEDURE BiElliptic }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE ONETANGENT
|
|  This PROCEDURE calculates the delta v's for a One Tangent transfer for either
|    circle to circle, or ellipse to ellipse.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    RInit       - Initial position magnitude     ER
|    RFinal      - Final position magnitude       ER
|    eInit       - Eccentricity of first orbit
|    eFinal      - Eccentricity of final orbit
|    NuInit      - True Anomaly of first orbit    0 or Pi rad
|    Nu2         - True Anomaly of second orbit   Same quad as NuInit, rad
|    NuFinal     - True Anomaly of final orbit    0 or Pi rad
|
|  OutPuts       :
|    DeltaVa     - Change in velocity at point a  ER / TU
|    DeltaVb     - Change in velocity at point b  ER / TU
|    DtTU        - Time of Flight for the transf  TU
|
|  Locals        :
|    SME1        - Mech Energy of first orbit     ER2 / TU
|    SME2        - Mech Energy of transfer orbit  ER2 / TU
|    SME3        - Mech Energy of final orbit     ER2 / TU
|    VInit       - Velocity of first orbit at a   ER / TU
|    vTransa     - Velocity of trans orbit at a   ER / TU
|    vTransb     - Velocity of trans orbit at b   ER / TU
|    VFinal      - Velocity of final orbit at b   ER / TU
|    aInit       - Semimajor axis of first orbit  ER
|    aTrans      - Semimajor axis of Trans orbit  ER
|    aFinal      - Semimajor axis of final orbit  ER
|    E           - Ecc anomaly of trans at b      rad
|    Ratio       - Ratio of initial to final
|                    orbit radii
|
|  Coupling      :
|    ATAN2       - Arc tangent rountine that solves quadrant ambiguities
|
|  References    :
|    Vallado       2007, 335, Alg 38, Ex 6-3
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE OneTangent         ( RInit,RFinal,eInit,eFinal,
                               NuInit,NuTran                         : EXTENDED;
                               VAR Deltava,Deltavb,DtTU              : EXTENDED );
   VAR
     EAInit,VInit,VTrana,VTranb,VFinal, eTran,aInit,aTran,aFinal, fpaTranb,fpaFinal, E, Sinv,Cosv,
     Ratio : EXTENDED;
   BEGIN
     { --------------------  Initialize values   ------------------- }
     DeltaVa:= 0.0;
     DeltaVb:= 0.0;
     DtTU    := 0.0;
     Ratio  := rInit/rFinal;
     IF ABS(NuInit) < 0.01 THEN { check 0 or 180 }
       BEGIN
         eTran  := ( Ratio-1.0 ) / ( COS(NuTran)-Ratio );  { init at perigee }
         EAInit:= 0.0;
       END
       ELSE
       BEGIN
         eTran  := ( Ratio-1.0 ) / ( COS(NuTran)+Ratio ); { init at apogee  }
         EAInit:= Pi;
       END;
     IF eTran >= 0.0 THEN
       BEGIN
         aInit := (rInit*(1.0+eInit*COS(NuInit))) / (1.0 - eInit*eInit );
         aFinal:= (rFinal*(1.0+eFinal*COS(NuTran))) / (1.0 - eFinal*eFinal );
                      { nutran is used since it = nufinal!! }
aInit := rinit;
afinal:= rfinal;
         IF ABS( eTran-1.0 ) > 0.000001 THEN
             IF ABS(NuInit) < 0.01 THEN { check 0 or 180 }
                 aTran := (rInit*(1.0+eTran*COS(NuInit))) / (1.0 - eTran*eTran ) { per }
               ELSE
{                 aTran := (rInit*(1.0+eTran*COS(NuInit))) / (1.0 + eTran*eTran )  apo }
                 aTran:= RInit/(1.0 + eTran)
           ELSE
           BEGIN
             aTran := 999999.9;  { Infinite for Parabolic orbit }
           END;

         { -----------------  Find Delta v at point a  ----------------- }
         VInit  := SQRT( 2.0/rInit - (1.0/aInit) );
         VTrana := SQRT( 2.0/rInit - (1.0/aTran) );
         DeltaVa:= ABS( VTrana - VInit );

         { -----------------  Find Delta v at point b  ----------------- }
         VFinal  := SQRT( 2.0/rFinal - (1.0/aFinal) );
         VTranb  := SQRT( 2.0/rFinal - (1.0/aTran) );
         fpaTranb:= ARCTAN( ( eTran*SIN(NuTran) ) / ( 1.0 + eTran*COS(NuTran) ) );
         fpaFinal:= ARCTAN( ( eFinal*SIN(NuTran) ) / ( 1.0 + eFinal*COS(NuTran) ) );
         DeltaVb := SQRT( VTranb*VTranb + VFinal*VFinal
                          - 2.0*VTranb*VFinal*COS( fpaTranb-fpaFinal ) );

         { ----------------  Find Transfer Time of Flight  ------------- }
         IF eTran < 0.99999 THEN
           BEGIN
             Sinv:= ( SQRT( 1.0-eTran*eTran )*SIN(NuTran) )
                    / ( 1.0 + eTran*COS(NuTran) );
             Cosv:= ( eTran+COS(NuTran) ) / ( 1.0 + eTran*COS(NuTran) );
             E   := ATAN2( Sinv,Cosv );
             DtTU:= SQRT( aTran*aTran*aTran ) *
                        ( E - eTran*SIN(E) - (EAInit - ETran*SIN(EAInit)) );
           END
           ELSE
             IF ABS( eTran-1.0 ) < 0.000001 THEN
                 { Parabolic DtTU }
               ELSE
                 { Hyperbolic DtTU }

           IF Show IN ['Y','S'] THEN
             BEGIN
               WriteLn( FileOut,' R-1 ',Ratio:10:6 );
               WriteLn( FileOut,'aInit  ',aInit:10:6,' ER' );
               WriteLn( FileOut,'aTran  ',aTran:10:6,' ER' );
               WriteLn( FileOut,'aFinal ',aFinal:10:6,' ER' );
               WriteLn( FileOut,'VInit  ', vInit:10:6,' VTrana ',VTrana:10:6,' DVa ',DeltaVa:10:6 );
               WriteLn( FileOut,' fpaTtran ',fpaTranb*57.2955:10:6,' fpaFinal = ',fpaFinal*57.2955:10:6 );
               WriteLn( FileOut,'Vtranb ',vTranb:10:6,' VFinal  ', vFinal:10:6,' DVb ',DeltaVb:10:6 );
               WriteLn( FileOut,'Total DV  ',(DeltaVa+DeltaVb):10:6,(Deltava+Deltavb)*7.90536599:12:7,' ER/TU' );
               WriteLn( FileOut,'Total DtTU ',DtTU:11:6,DtTU*13.44685115881:13:6,' min',DtTU*13.44685115881/60:11:6,' hrs' );
               WriteLn( FileOut,' eTran   ',eTran:10:6,' E = ',E*57.2955:10:6,' aTran ',aTran:10:6 );
             END;
       END
       ELSE
         WriteLn( 'The one tangent burn is not possible for this case ' );
   END;  { PROCEDURE OneTangent }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE IONLYCHG
|
|  This PROCEDURE calculates the delta v's for a change in inclination only.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    DeltaI      - Change in inclination          rad
|    VInit       - Initial velocity vector        ER/TU
|    fpa         - Flight path angle              rad
|
|  OutPuts       :
|    DeltaVionly - answer
|
|  Locals        :
|    None.
|
|  Coupling      :
|    None.
|
|  References    :
|    Vallado       2007, 346, Alg 39, Ex 6-4
|
 ----------------------------------------------------------------------------- }

PROCEDURE IOnlyChg             ( Deltai,VInit,fpa                    : EXTENDED;
                                 VAR DeltaViOnly                     : EXTENDED );
   BEGIN
     DeltaViOnly := 2.0*VInit*COS(fpa)*SIN(0.5*Deltai);
   END;  { PROCEDURE IOnlyChg }

{ ------------------------------------------------------------------------------
|
|                           PROCEDURE NODEONLYCHG
|
|  This PROCEDURE calculates the delta v's for a change in longitude of
|    ascending node only.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    DeltaOmega  - Change in Node                 Rad
|    ecc         - Ecc of first orbit
|    VInit       - Initial velocity vector        ER/TU
|    fpa         - Flight path angle              rad
|    Incl        - Inclination                    rad
|
|
|  OutPuts       :
|    iFinal      - Final inclination              rad
|    Deltav      - Change in velocity             ER/TU
|
|  Locals        :
|    VFinal      - Final velocity vector          ER/TU
|    ArgLat      - Argument of latitude           rad
|    ArgLat1     - Final Argument of latitude     rad
|    NuInit      - Initial true anomaly           rad
|    Theta       -
|
|  Coupling      :
|    ASIN      - Arc sine FUNCTION
|    ACOS      - Arc cosine FUNCTION
|
|  References    :
|    Vallado       2007, 349, Alg 40, Ex 6-5
|
 ----------------------------------------------------------------------------- }

PROCEDURE NodeOnlyChg          ( iInit,ecc,DeltaOmega,VInit,fpa,Incl : EXTENDED;
                                 VAR iFinal,DeltaV                   : EXTENDED );
   CONST
     Rad : EXTENDED = 57.29577951308230;
   VAR
     vfinal,ArgLat,ArgLat1,NuInit,Theta : EXTENDED;
   BEGIN
     IF ABS(ecc) > 0.00000001 THEN
       BEGIN
         { ------------------------- Elliptical --------------------- }
         Theta := ARCTAN( SIN(iInit)*TAN(DeltaOmega) );
         iFinal:= ASIN( SIN(Theta)/SIN(DeltaOmega) );
         DeltaV:= 2.0*VInit*COS(fpa)*SIN(0.5*Theta);

         ArgLat := Pi*0.5; { set at 90 deg }
         ArgLat1:= ACOS( COS(incl)*SIN(incl)*(1.0-COS(DeltaOmega))
                   / SIN(Theta) );

         IF Show = 'Y' THEN
           WriteLn( FileOut,'dv 90 deg',deltav:11:7,deltav*7.9053659986:11:7,'km/s Theta ',
                  Theta*Rad:11:7,' ifinal ',ifinal*rad:11:7,' ArgLat',ArgLat*rad:11:6 );
       END
       ELSE
       BEGIN
         { -------------------------- Circular ---------------------- }
         theta := ACOS( SQR(COS(iinit))+SQR(SIN(iinit))*COS(DeltaOmega) );
         DeltaV:= 2.0*VInit*SIN(0.5*Theta);

         ArgLat := ACOS( TAN(iInit)*(COS(DeltaOmega)-COS(Theta))
                           / SIN(Theta) );
         ArgLat1:= ACOS( COS(incl)*SIN(incl)*(1.0-COS(DeltaOmega))
                           / SIN(Theta) );

         IF Show = 'Y' THEN
           WriteLn( FileOut,'dv cir   ',deltav:11:7,deltav*7.9053659986:11:7,'km/s Theta ',
                          Theta*Rad:11:7,'ArgLat =',ArgLat*rad:11:6,'ArgLat1 =',ArgLat1*rad:11:6 );
       END;

   END;  { PROCEDURE NodeOnlyChg }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE IandNodeChg
|
|  This PROCEDURE calculates the delta v's for a change in inclination and
|    longitude of ascending node.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    VInit       - Initial velocity vector        ER/TU
|    iInit       - Initial inclination            rad
|    fpa         - Flight path angle              rad
|    DeltaOmega  - Change in Node                 Rad
|    DeltaI      - Change in inclination          Rad
|    RFinal      - Final position magnitude       ER
|
|  OutPuts       :
|    iFinal      - Final inclination              rad
|    Deltav      - Change in velocity             ER/TU
|
|  Locals        :
|    ArgLat      - Argument of latitude           rad
|    ArgLat1     - Final Argument of latitude     rad
|    Theta       -
|
|  Coupling      :
|    ACOS      - Arc cosine FUNCTION
|
|  References    :
|    Vallado       2007, 350, Alg 41, Ex 6-6
|
 ----------------------------------------------------------------------------- }

PROCEDURE IAndNodeChg          ( iInit,DeltaOmega,Deltai,VInit,fpa   : EXTENDED;
                                 VAR DeltaV,iFinal                   : EXTENDED );
   CONST
      Rad : EXTENDED = 57.29577951308230;
   VAR
      ArgLat,ArgLat1,Theta : EXTENDED;
   BEGIN
     iFinal:= iInit - Deltai;
     theta := ACOS( COS(iinit)*COS(ifinal) +
                      SIN(iinit)*SIN(ifinal)*COS(DeltaOmega) );
     DeltaV:= 2.0*VInit*COS(fpa)*SIN(0.5*Theta);

     ArgLat := ACOS( (SIN(ifinal)*COS(DeltaOmega) -
                       COS(Theta)*SIN(iinit)) / (SIN(Theta)*COS(iinit)) );
     ArgLat1:= ACOS( (COS(iInit)*SIN(iFinal) -
                       SIN(IInit)*COS(iFinal)*COS(DeltaOmega)) / SIN(Theta) );

     IF Show = 'Y' THEN
        WriteLn( FileOut,'proce  dv ',deltav:11:7,deltav*7.9053659986:11:7,'km/s Theta ',
              Theta*Rad:11:7,' ArgLat =',ArgLat*rad:11:6,' ArgLat1 =',ArgLat1*rad:11:6,
              'IF ',ifinal*Rad:11:6 );
   END;  { PROCEDURE IandNodeChg }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE MINCOMBINEDPLANECHG
|
|  This PROCEDURE calculates the delta v's and the change in inclination
|    necessary for the minimum change in velocity when traveling between two
|    non-coplanar orbits.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    RInit       - Initial position magnitude     ER
|    RFinal      - Final position magnitude       ER
|    eInit       - Ecc of first orbit
|    e2          - Ecc of trans orbit
|    eFinal      - Ecc of final orbit
|    NuInit      - True Anomaly of first orbit    0 or Pi rad
|    NuFinal     - True Anomaly of final orbit    0 or Pi rad
|    iInit       - Incl of the first orbit        rad
|    iFinal      - Incl of the second orbit       rad
|
|  OutPuts       :
|    Deltai1     - Amount of incl chg req at a    rad
|    DeltaVa     - Change in velocity at point a  ER / TU
|    DeltaVb     - Change in velocity at point b  ER / TU
|    DtTU        - Time of Flight for the trans   TU
|    NumIter     - Number of iterations
|
|  Locals        :
|    SME1        - Mech Energy of first orbit     ER2 / TU
|    SME2        - Mech Energy of transfer orbit  ER2 / TU
|    SME3        - Mech Energy of final orbit     ER2 / TU
|    VInit       - Velocity of first orbit at a   ER / TU
|    vTransa     - Velocity of trans orbit at a   ER / TU
|    vTransb     - Velocity of trans orbit at b   ER / TU
|    VFinal      - Velocity of final orbit at b   ER / TU
|    aInit       - Semimajor axis of first orbit  ER
|    aTrans      - Semimajor axis of Trans orbit  ER
|    aFinal      - Semimajor axis of final orbit  ER
|    e2          - Eccentricity of second orbit
|
|  Coupling      :
|    POWER       - Raise a base to a power
|    ASIN      - Arc sine routine
|
|  References    :
|    Vallado       2007, 355, Alg 42, Table 6-3
|
 ----------------------------------------------------------------------------- }

PROCEDURE MinCombinedPlaneChg( RInit,RFinal,eInit,eFinal,NuInit,
                               NuFinal,iInit,iFinal                  : EXTENDED;
                               VAR Deltai,Deltai1,DeltaVa,DeltaVb,
                                   DtTU                              : EXTENDED );
   CONST
     Rad : EXTENDED = 57.29577951308230;
   VAR
 numiter:INTEGER;
 deltainew,
      dvold,temp,TDi, SME1,SME2,SME3, VInit,V1t,V3t,VFinal, a1,a2,a3 : EXTENDED;
   BEGIN
     { --------------------  Initialize values   -------------------- }
     a1  := (RInit*(1.0+eInit*COS(NuInit))) / (1.0 - eInit*eInit );
     a2  := 0.5 * (RInit+RFinal);
     a3  := (RFinal*(1.0+eFinal*COS(NuFinal))) / (1.0 - eFinal*eFinal );
     SME1:= -1.0 / (2.0*a1);
     SME2:= -1.0 / (2.0*a2);
     SME3:= -1.0 / (2.0*a3);

     { ----------- Find velocities -------- }
     VInit := SQRT( 2.0*( (1.0/RInit) + SME1 ) );
     V1t   := SQRT( 2.0*( (1.0/RInit) + SME2 ) );

     VFinal:= SQRT( 2.0*( (1.0/RFinal) + SME3 ) );
     V3t   := SQRT( 2.0*( (1.0/RFinal) + SME2 ) );

     { ----------- Find the optimum change of inclination ----------- }
     TDi := iFinal-iInit;

     Temp:= (1.0/TDi) * ARCTAN( (POWER(RFinal/RInit,1.5)-COS(TDi)) / SIN(TDi) );
     Temp:= (1.0/TDi) * ARCTAN( SIN(TDi) / (POWER(RFinal/RInit,1.5)+COS(TDi)) );

     DeltaVa:= SQRT( V1t*V1t + VInit*VInit - 2.0*V1t*VInit*COS(Temp*Tdi) );
     DeltaVb:= SQRT( V3t*V3t + VFinal*VFinal - 2.0*V3t*VFinal*COS(TDi*(1.0-Temp)) );

     Deltai := Temp*TDi;
     Deltai1:= TDi*(1.0-Temp);

     { ----------------  Find Transfer Time of Flight  -------------- }
     DtTU:= Pi * SQRT( A2*A2*A2 );

     IF Show = 'Y' THEN
       BEGIN
         DVold:= ABS(V1t-VInit) + SQRT( V3t*V3t + VFinal*VFinal - 2.0*V3t*VFinal*COS(TDi) );
         WriteLn( FileOut,'s = ',temp:11:7,' this uses Di in rad ' );
         WriteLn( FileOut,'RInit ',RInit:14:7,RInit*6378.137:14:7,' RFinal ',RFinal:14:7,RFinal*6378.137:14:7 );
         WriteLn( FileOut,'Deltai1 ',Deltai*Rad:13:7,Deltai1*Rad:13:7 );
         WriteLn( FileOut,'DeltaVa ',DeltaVa:13:7,'DeltaVb ',DeltaVb:13:7,' ER/TU ' );
         WriteLn( FileOut,'DeltaVa ',DeltaVa*7.905365998:13:7,'DeltaVb ',
                                     DeltaVb*7.905365998:13:7,' km/s ' );
         WriteLn( FileOut,1000*(Deltava+Deltavb)*7.905365998:13:7,' m/s' );
         WriteLn( FileOut,'Dv old way ',1000*dvold*7.905365998:13:7,' m/s' );
         WriteLn( FileOut,'DtTU     ',DtTU*13.446851158:13:7,' min' );
       END;

     { ----- Iterate to find the optimum change of inclination ----- }
     DeltaiNew  := Deltai;         { 1st guess, 0.01 to 0.025 seems good }
     Deltai1    := 100.0;              { IF going to smaller orbit, should be}
     NumIter    := 0;                  { 1.0 - 0.025! }

     WHILE ABS(DeltaiNew-Deltai1) > 0.000001 DO
       BEGIN
         Deltai1:= DeltaiNew;
         DeltaVa:= SQRT( V1t*V1t + VInit*VInit - 2.0*V1t*VInit* COS(Deltai1) );

         DeltaVb:= SQRT( V3t*V3t + VFinal*VFinal - 2.0*V3t*VFinal* COS(TDi-Deltai1) );

         DeltaiNew:= ASIN( (DeltaVa*VFinal*V3t*SIN(TDi-Deltai1)) / (VInit*V1t*DeltaVb) );
         INC(NumIter);
       END;  { While ABS() }
     Writeln( fileout,'Iter Di ',deltai1*Rad:14:6,'ø',numiter:3,
             (deltava+deltavb)*7905.365998:13:7 );


   END;  { PROCEDURE MinCombinedPlaneChg }
{}
{
|    Vallado       2007, 355, Ex 6-7
}
PROCEDURE CombinedPlaneChg   ( RInit,RFinal,eInit,e2,eFinal,
                               NuInit,Nu2a,Nu2b,NuFinal,Deltai              : EXTENDED;
                               VAR Deltava,Deltavb,DtTU               : EXTENDED );
   VAR
     SME1,SME2,SME3, VInit,vTransa,vTransb,VFinal, a1,a2,a3,fpa1,fpa2a,fpa2b,fpa3,
     E,Eo,Sinv,Cosv   : EXTENDED;
   BEGIN
     { --------------------  Initialize values   ------------------- }
     a1 := (RInit*(1.0+eInit*COS(NuInit))) / (1.0 - eInit*eInit );
     IF ABS( e2-1.0 ) > 0.000001 THEN
       BEGIN
         a2  := (RInit*(1.0+e2*COS(Nu2a))) / (1.0 - e2*e2 );
         SME2:= -1.0 / (2.0*a2);
       END
       ELSE
       BEGIN
         a2  := 999999.9;  { Undefined for Parabolic orbit }
         SME2:= 0.0;
       END;
     a3 := (RFinal*(1.0+eFinal*COS(NuFinal))) / (1.0 - eFinal*eFinal );
     SME1:= -1.0 / (2.0*a1);
     SME3:= -1.0 / (2.0*a3);

     { -----------------  Find Delta v at point a  ----------------- }
     VInit := SQRT( 2.0*( (1.0/RInit) + SME1 ) );
     vTransa:= SQRT( 2.0*( (1.0/RInit) + SME2 ) );
     fpa2a:= ARCTAN( ( e2*SIN(Nu2a) ) / ( 1.0 + e2*COS(Nu2a) ) );
     fpa1 := ARCTAN( ( eInit*SIN(NuInit) ) / ( 1.0 + eInit*COS(NuInit) ) );
     DeltaVa:= SQRT( vTransa*vTransa + VInit*VInit - 2.0*vTransa*VInit*
                     ( SIN(fpa2a)*SIN(fpa1)+COS(fpa2a)*COS(fpa1)*COS(Deltai)));

     { -----------------  Find Delta v at point b  ----------------- }
     VFinal := SQRT( 2.0*( (1.0/RFinal) + SME3 ) );
     vTransb:= SQRT( 2.0*( (1.0/RFinal) + SME2 ) );
     fpa2b:= ARCTAN( ( e2*SIN(Nu2b) ) / ( 1.0 + e2*COS(Nu2b) ) );
     fpa3 := ARCTAN( ( eFinal*SIN(NuFinal) ) / ( 1.0 + eFinal*COS(NuFinal) ) );
     DeltaVb:= SQRT( vTransb*vTransb + VFinal*VFinal - 2.0*vTransb*VFinal*
                     ( SIN(fpa2b)*SIN(fpa3)+COS(fpa2b)*COS(fpa3)*COS(Deltai)));

     { ----------------  Find Transfer Time of Flight  ------------- }
     Sinv:= ( SQRT( 1.0-e2*e2 )*SIN(Nu2b) ) / ( 1.0 + e2*COS(Nu2b) );
     Cosv:= ( e2+COS(Nu2b) ) / ( 1.0 + e2*COS(Nu2b) );
     E:= ATAN2( Sinv,Cosv );
     Sinv:= ( SQRT( 1.0-e2*e2 )*SIN(Nu2a) ) / ( 1.0 + e2*COS(Nu2a) );
     Cosv:= ( e2+COS(Nu2a) ) / ( 1.0 + e2*COS(Nu2a) );
     Eo:= ATAN2( Sinv,Cosv );
     DtTU:= SQRT( A2*A2*A2 ) * ( (E - e2*SIN(E)) - (Eo - e2*SIN(Eo)) );

   END;  { PROCEDURE CombinedPlaneChg }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE RENDEZVOUS
|
|  This PROCEDURE calculates parameters for a Hohmann transfer rendezvous.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Rcs1        - Radius of circular orbit int   ER
|    Rcs2        - Radius of circular orbit tgt   ER
|    eInit       - Ecc of first orbit
|    eFinal      - Ecc of final orbit
|    NuInit      - True Anomaly of first orbit    0 or Pi rad
|    NuFinal     - True Anomaly of final orbit    0 or Pi rad
|    PhaseI      - Initial phase angle (Tgt-Int)  +(ahead) or -(behind) rad
|    NumRevs     - Number of revs to wait
|    kTgt        -
|    kInt        -
|
|  OutPuts       :
|    PhaseF      - Final Phase Angle              rad
|    WaitTime    - Wait before next intercept opp TU
|    Deltav      - Change in velocity             ER/TU
|
|  Locals        :
|    DtTUTrans   - Time of flight of trans orbit  TU
|    ATrans      - Semimajor axis of trans orbit  ER
|    AngVelTgt   - Angular velocity of target     rad / TU
|    AngVelInt   - Angular velocity of int        rad / TU
|    LeadAng     - Lead Angle                     rad
|
|  Coupling      :
|    POWER       - Raise a base to a power
|
|  References    :
|    Vallado       2007, 364, Alg 44, Alg 45, Ex 6-8, Ex 6-9
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE Rendezvous         ( Rcs1,Rcs3,PhaseI,eInit,eFinal,
                               NuInit,NuFinal                        : EXTENDED;
                               kTgt,kInt                             : INTEGER;
                               VAR PhaseF,WaitTime,DeltaV            : EXTENDED );
   CONST
     TwoPi : EXTENDED =  6.28318530717959;
     Pi    : EXTENDED =  3.14159265358979;
     Rad   : EXTENDED = 57.29577951308230;
   VAR
     PeriodTrans,Rp,DtTUTrans,LeadAng,aTrans,AngVelTgt,AngVelInt,
     a1,a2,a3,VInit,vTransa,VFinal,vTransb,SME1,SME2,SME3,DeltaVa,DeltaVb,VInt,VTrans   : EXTENDED;
   BEGIN
     ATrans    := (Rcs1 + Rcs3) / 2.0;
     DtTUTrans := Pi*SQRT( ATrans*ATrans*ATrans );
     AngVelInt := 1.0 / ( SQRT(Rcs1*Rcs1*Rcs1) );
     AngVelTgt := 1.0 / ( SQRT(Rcs3*Rcs3*Rcs3) );
     VInt      := SQRT( 1.0/Rcs1 );

     { ---------- Check for satellites in the same orbits ----------- }
     IF ABS( AngVelInt - AngVelTgt ) < 0.000001 THEN
       BEGIN
         PeriodTrans:= ( kTgt*TwoPi + PhaseI ) / AngVelTgt;
         aTrans     := POWER( (PeriodTrans/(TwoPi*kInt)),2.0/3.0 );
         Rp         := 2.0*aTrans - Rcs1;
         IF Rp < 1.0 THEN
             WriteLn( ' Error - The transfer orbit intersects the Earth ' );
         VTrans  := SQRT( (2.0/Rcs1) - (1.0/aTrans) );
         DeltaV  := 2.0*(VTrans-VInt);
         WaitTime:= 0.0;
{ writeln( fileout,'tpha ',periodtrans:11:7,' vint ',vint:11:7,' phi ',phasei*rad:11:7 );}
 phasef:= phasei;
 waittime:= periodtrans;
 leadang:= 0.0;
       END
       ELSE
       BEGIN
         LeadAng := AngVelTgt * DtTUTRans;
         PhaseF  := LeadAng - Pi;
         WaitTime:= ( PhaseF - PhaseI + 2.0*Pi*kTgt ) / ( AngVelInt - AngVelTgt );

         a1  := (rcs1*(1.0+eInit*COS(NuInit))) / (1.0 - eInit*eInit );
         a2  := ( Rcs1 + Rcs3 ) / 2.0;
         a3  := (rcs3*(1.0+eFinal*COS(NuFinal))) / (1.0 - eFinal*eFinal );
         SME1:= -1.0 / (2.0*a1);
         SME2:= -1.0 / (2.0*a2);
         SME3:= -1.0 / (2.0*a3);
     { -----------------  Find Delta v at point a  ------------------ }
         VInit := SQRT( 2.0*( (1.0/Rcs1) + SME1 ) );
         vTransa:= SQRT( 2.0*( (1.0/Rcs1) + SME2 ) );
         DeltaVa:= ABS( vTransa - VInit );

     { -----------------  Find Delta v at point b  ------------------ }
         VFinal := SQRT( 2.0*( (1.0/Rcs3) + SME3 ) );
         vTransb:= SQRT( 2.0*( (1.0/Rcs3) + SME2 ) );
         DeltaVb:= ABS( VFinal - vTransb );
         DeltaV:= DeltaVa + DeltaVb;
       END;

{}     IF Show IN ['Y','S'] THEN
{}       BEGIN
{}         Writeln( FileOut,'    A transfer  = ',ATRans:12:8,  ' ER ' );
{}         Writeln( FileOut,'    DtTU Transfer= ',DtTUTRans:12:8,' TU ',DtTUTrans*13.44685115:11:7 );
{}         WriteLn( FileOut,'    AngVelTgt   = ',AngVelTgt:12:8,  ' rad/TU kTgt ',kTgt:3,' kInt ',kInt:3 );
{}         WriteLn( FileOut,'    AngVelInt   = ',AngVelInt:12:8,  ' rad/TU' );
{}         Writeln( FileOut,'    Lead Angle  = ',LeadAng*Rad:12:8,' ø' );
{}       END;

   END;  { PROCEDURE Rendezvous }
{}
{ -----
|   Vallado       2007, 370, Alg 46, Ex 6-10

------ }
PROCEDURE NonCoplanarRendz   ( PhaseNew,Deltai,Delta2Node,LonTrue,
                               RInt,RTgt                             : EXTENDED;
                               kTgt,kInt                             : INTEGER;
                               VAR TTrans,TPhase,DVPhase,
                               DVTrans1,DVTrans2                     : EXTENDED );
   CONST
      TwoPi : EXTENDED =  6.28318530717959;
      Pi    : EXTENDED =  3.14159265358979;
      Rad   : EXTENDED = 57.29577951308230;
   VAR
     AngVelInt,AngVelTgt,atrans,aphase,lead,leadNew,TNode,LonTrueNew,
     VInt,VTgt,VPhase,VTrans1,VTrans2    : EXTENDED;

   BEGIN
     AngVelInt:= SQRT( 1.0/(RInt*RInt*RInt) );
     AngVelTgt:= SQRT( 1.0/(RTgt*RTgt*RTgt) );
     ATrans   := (RInt + RTgt) * 0.5;
     TTrans := Pi*SQRT( ATrans*ATrans*ATrans );

     Lead := AngVelTgt * TTRans;

     TNode:= Delta2Node/AngVelInt;

     LonTrueNew:= LonTrue + AngVelTgt*TNode;
{fix     PhaseNew:= 13.5/Rad;}
     LeadNew:= Pi + PhaseNew;

     TPhase:= (LeadNew - Lead + TwoPi*kTgt) / AngVelTgt;

     aPhase := POWER( (TPhase/(TwoPi*kInt)),2.0/3.0 );

     { -----------------  Find Deltav's  ----------------- }
     VInt:= SQRT(1.0/RInt);
     VPhase:= SQRT(2.0/RInt - 1.0/aPhase);
     DVPhase:= VPhase - VInt;

     VTrans1:= SQRT(2.0/RInt - 1.0/aTrans);
     DVTrans1:= VTrans1 - VPhase;

     VTrans2:= SQRT(2.0/RTgt - 1.0/aTrans);
     VTgt:= SQRT(1.0/RTgt);
     DVTrans2:= SQRT(VTgt*VTgt + VTrans2*VTrans2 - 2.0*VTgt*VTrans2*COS(Deltai));

     IF Show IN ['Y','N'] THEN
       BEGIN
         Writeln( FileOut,'    A trans  = ',ATRans:12:8,  ' ER ' );
         Writeln( FileOut,'    Lead Angle N= ',LeadNew*Rad:12:8,' ø' );
         WriteLn( FileOut,'    Time to Node= ',TNode:12:8,' TU' );
         Writeln( FileOut,'    Angle Init  = ',Lead*Rad:12:8,' ø' );
         WriteLn( FileOut,'    Phasenew    = ',Phasenew*Rad:12:8,' ø');
         Writeln( FileOut,'    Lead Angle N= ',LeadNew*Rad:12:8,' ø' );
         Writeln( FileOut,'    DtTU Phaseing= ',TPhase:12:8,' TU ' );
         Writeln( FileOut,'    A phase  = ',APhase:12:8,  ' ER ' );
         WriteLn( FileOut,'    vint tgt = ',vint:11:7,vtgt:11:7 );

         WriteLn( FileOut,'    vtrans1,2 = ',vtrans1:11:7,vtrans2:11:7,' vp',vphase:11:7 );

         Writeln( FileOut,'    DtTU Transfer= ',TTRans:12:8,' TU ' );
         WriteLn( FileOut,'    AngVelTgt   = ',AngVelTgt:12:8,  ' rad/TU kTgt ',kTgt:3,' kInt ',kInt:3 );
         WriteLn( FileOut,'    AngVelInt   = ',AngVelInt:12:8,  ' rad/TU' );
         WriteLn( FileOut,'    LonTrueNew  = ',LonTrueNew*Rad:12:8,' ø');
         Writeln( FileOut,'    DeltaV  ph  = ',DVPhase:12:8,DVPhase*7.905365:12:8 );
         Writeln( FileOut,'    DeltaV  t1  = ',DVTrans1:12:8,DVTrans1*7.905365:12:8 );
         Writeln( FileOut,'    DeltaV  t2i = ',DVTrans2:12:8,DVTrans2*7.905365:12:8 );
       END;
   END;  { PROCEDURE NonCoplanarRendz }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE HILLSR
|
|  This PROCEDURE calculates various position information for Hills equations.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    R           - Initial Position vector of INT ER
|    V           - Initial Velocity Vector of INT ER / TU
|    Alt         - Altitude of TGT satellite      ER
|    DtTU        - Desired Time                   TU
|
|  Outputs       :
|    RInit       - Final Position vector of INT   ER
|    VInit       - Final Velocity Vector of INT   ER / TU
|
|  Locals        :
|    nt          - Angular velocity times time    rad
|    Omega       -
|    Sinnt       - Sine of nt
|    Cosnt       - Cosine of nt
|    Radius      - Magnitude of range vector      ER
|
|  Coupling      :
|    MAG         - Magnitude of a vector
|
|  References    :
|    Vallado       2007, 397, Alg 47, Ex 6-14
|
 ----------------------------------------------------------------------------- }

PROCEDURE HillsR             ( r,v                                   : Vector;
                               Alt,DtTU                              : EXTENDED;
                               VAR RInit,VInit                       : Vector   );
   VAR
     SinNt,CosNt,Omega,nt,Radius       : EXTENDED;
   BEGIN
     { ---------------- Initialize the orbit elements -------------- }
     Radius:= 1.0 + Alt;
     Omega := SQRT( 1.0 / (Radius*Radius*Radius) );
     nt    := Omega*DtTU;
     CosNt := COS( nt );
     SinNt := SIN( nt );

     { ---------------- Determine new positions  ------------------- }
     RInit[1]:= ( V[1]/Omega ) * SinNt -
             ( (2.0*V[2]/Omega) + 3.0*R[1] ) * CosNt +
             ( (2.0*V[2]/Omega) + 4.0*R[1] );
     RInit[2]:= ( 2.0*V[1]/Omega ) * CosNt +
             ( (4.0*V[2]/Omega) + 6.0*R[1] ) * SinNt +
             ( R[2] - (2.0*V[1]/Omega) ) -
             ( 3.0*V[2] + 6.0*Omega*R[1] )*DtTU;
     RInit[3]:= R[3]*CosNt + (V[3]/Omega)*SinNt;
     MAG( RInit );

     { ---------------- Determine new velocities  ------------------ }
     VInit[1]:= V[1]*CosNt + (2.0*V[2]+3.0*Omega*R[1])*SinNt;
     VInit[2]:= -2.0*V[1]*SinNt + (4.0*V[2]+6.0*Omega*R[1])*CosNt -
             (3.0*V[2]+6.0*Omega*R[1]);
     VInit[3]:= -R[3]*Omega*SinNt + V[3]*CosNt;
     MAG( VInit );

   END;  { PROCEDURE HillsR }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE HILLSV
|
|  This PROCEDURE calculates initial velocity for Hills equations.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    R           - Initial Position vector of INT ER
|    Alt         - Altitude of TGT satellite      ER
|    DtTU        - Desired Time                   TU
|
|  Outputs       :
|    V           - Initial Velocity Vector of INT ER / TU
|
|  Locals        :
|    Numer       -
|    Denom       -
|    nt          - Angular velocity times time    rad
|    Omega       -
|    Sinnt       - Sine of nt
|    Cosnt       - Cosine of nt
|    Radius      - Magnitude of range vector      ER
|
|  Coupling      :
|    None.
|
|  References    :
|    Vallado       2007, 410, Eq 6-60, Ex 6-15
|
 ----------------------------------------------------------------------------- }

PROCEDURE HillsV             ( r                                     : Vector;
                               Alt,DtTU                              : EXTENDED;
                               VAR V                                 : Vector   );

   VAR
     Numer,Denom,SinNt,CosNt,Omega,nt,Radius       : EXTENDED;
   BEGIN
     { --------------- Initialize the orbit elements --------------- }
     Radius:= 1.0 + Alt;
     Omega := SQRT( 1.0 / (Radius*Radius*Radius) );
     nt    := Omega*DtTU;
     CosNt := COS( nt );
     SinNt := SIN( nt );

     { ---------------- Determine initial Velocity ----------------- }
     Numer:= ( (6.0*r[1]*(nt-SinNt)-r[2])*Omega*SinNt-2.0*Omega*r[1]*
               (4.0-3.0*CosNt)*(1.0-CosNt) );
     Denom:= (4.0*SinNt-3.0*nt)*SinNt + 4.0*( 1.0-CosNt )*( 1.0-CosNt );

     IF ABS( Denom ) > 0.000001 THEN
         V[2]:= Numer / Denom
       ELSE
         V[2]:= 0.0;
     IF ABS( SinNt ) > 0.000001 THEN
         V[1]:= -( Omega*r[1]*(4.0-3.0*CosNt)+2.0*(1.0-CosNt)*v[2] ) / ( SinNt )
       ELSE
         V[1]:= 0.0;
     V[3]:= -R[3]*Omega*Cot(nt);

   END;  { PROCEDURE HillsV }
{}
PROCEDURE IJK_RSW            ( VAR Rijk,Vijk                         : Vector;
                               R,S,W                                 : Vector;
                               Direction                             : Direct;
                               VAR Rrsw,Vrsw                         : Vector );
   VAR
     ROTMat : Matrix;
     i : INTEGER;
   BEGIN
     ROTMat:= NIL;
     InitMatrix( 3,3,1,1, RotMat );

     IF Direction = FROM THEN
       BEGIN
         { ------ Form Rotation Matrix ------ }
         FOR i:= 1 to 3 DO
           BEGIN
             AssignVal( ROTMat,i,1, R[i] );
             AssignVal( ROTMat,i,2, S[i] );
             AssignVal( ROTMat,i,3, W[i] );
           END;

         { ------- Do multiplication -------- }
         FOR i:= 1 to 4 DO
           BEGIN
             rijk[i]:= 0.0;
             vijk[i]:= 0.0;
           END;
         MatVecMult( ROTMat,Rrsw,3,3, Rijk );
         MatVecMult( ROTMat,Vrsw,3,3, Vijk );
         MAG( Rijk);
         MAG( vijk);
       END
       ELSE
       BEGIN
         { ------ Form Rotation Matrix ------ }
         FOR i:= 1 to 3 DO
           BEGIN
             AssignVal( ROTMat,1,i, R[i] );
             AssignVal( ROTMat,2,i, S[i] );
             AssignVal( ROTMat,3,i, W[i] );
           END;

         { ------- Do multiplication -------- }
         FOR i:= 1 to 4 DO
           BEGIN
             rrsw[i]:= 0.0;
             vrsw[i]:= 0.0;
           END;
         MatVecMult( ROTMat,Rijk,3,3, Rrsw );
         MatVecMult( ROTMat,Vijk,3,3, Vrsw );
         MAG( Rrsw);
         MAG( vrsw);
       END;

     DelMatrix( RotMat );
   END;  { PROCEDURE IJKRSW }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE COW2Hill
|
|  This PROCEDURE finds the equivalent relative motion vector given a geocentric
|    vector.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Rtgt        - Position vector of tgt         ER
|    Vtgt        - Velocity Vector of tgt         ER / TU
|    Rint        - Position vector of int         ER
|    Vint        - Velocity Vector of int         ER / TU
|
|  Outputs       :
|    RHill       - Position vector of int rel to
|                  target                         ER
|    VHill       - Velocity Vector of int rel to
|                  target                         ER / TU
|
|  Locals        :
|    None.
|
|
|
|
|
|
|  Coupling      :
|    CROSS       - Cross product of two vectors
|    NORM        - Unit vector
|    IJK_RSW     -
|    LNCOM2      - Linear combination of two scalars and two vectors
|    MAG         - Magnitude of a vector
|    ROT3        - Rotation about the 3rd axis
|
|  References    :
|    Vallado       2007, 413
|
 ----------------------------------------------------------------------------- }

PROCEDURE Cow2Hill           ( rtgt,vtgt,rint,vint                   : Vector;
                               VAR RHill,VHill                       : VECTOR );
   VAR
     i : INTEGER;
     r,s,w,rv,  rtemp,vtemp,rt,vt,ri,vi : Vector;
     Halfpi,angly, anglz, temp, LST,Lat, angl : EXTENDED;
   BEGIN
     { ---- Form RSW unit vectors for transformation ---- }
     NORM ( rtgt,  R );
     CROSS( rtgt,vtgt, RV );
     NORM ( RV,  W );
     CROSS( W,R, S );

     IJK_RSW( RTgt,VTgt,R,S,W, TOO, Rt,Vt );  { IJK to RSW }
     IJK_RSW( RInt,VInt,R,S,W, TOO, Ri,Vi );  { IJK to RSW }

     { ---- Determine z offset to correct vector ---- }
     IF ABS(ri[3]) > 0.0000001 THEN
       BEGIN
         anglz:= ARCTAN(ri[3]/rt[4]); { coord sys based at tgt }
         ROT2( Ri, -anglz, Ri );
         ROT2( Vi, -anglz, Vi ); { should be ROT2(a), but opp }
       END
       ELSE
         anglz:= 0.0;

     { ---- Determine y offset to correct vector ---- }
     IF ABS(ri[2]) > 0.0000001 THEN
       BEGIN
         angly:= ARCTAN(ri[2]/rt[4]); { should be ROT3(-a), but opp, but sign for later}
         ROT3( Ri, angly, Ri );
         ROT3( Vi, angly, Vi );
       END
       ELSE
         angly:= 0.0;

     { ---- Do all 3 here ---- }
{     LNCOM2( 1.0,-1.0,ri,rt, rHill );}
     LNCOM2( 1.0,-1.0,vi,vt, VHill );

     { ---- Now add in corrections ---- }
     RHill[1]:= ri[1] - rt[4{4if not do rotri or 1}];
     RHill[2]:= angly*ri[4];
     RHill[3]:= anglz*ri[4];

     MAG( RHill );
     MAG( VHill );
   END;  { PROCEDURE Cow2Hill }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE HILL2COW
|
|  This PROCEDURE finds the equivalent geocentric vector given the target and
|    relative motion vectors.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Rtgt        - Position vector of tgt         ER
|    Vtgt        - Velocity Vector of tgt         ER / TU
|    RHill       - Position vector of int rel to
|                  target                         ER
|    VHill       - Velocity Vector of int rel to
|                  target                         ER / TU
|
|  Outputs       :
|    Rint        - Position vector of int         ER
|    Vint        - Velocity Vector of int         ER / TU
|
|  Locals        :
|    None.
|
|
|
|
|
|
|  Coupling      :
|    IJKRSW      - Form translation matrix given position and velocity vectors
|    ROT2        - Rotation about the 2nd axis
|    ROT3        - Rotation about the 3rd axis
|    MAG         - Magnitude of a vector
|    NORM        - Unit vector
|    CROSS       - Cross product of two vectors
|
|  References    :
|    Vallado       2007, 414
|
 ----------------------------------------------------------------------------- }

PROCEDURE Hill2Cow           ( rTgtijk,vTgtijk,RHill,VHill           : VECTOR;
                               VAR rIntijk,vIntijk                   : Vector );
   VAR
     i : INTEGER;
     angly, anglz : EXTENDED;
     rtem,vtem, RTgtrsw,VTgtrsw,rv,R,S,W : Vector;
   BEGIN
     { ---- Form RSW unit vectors for transformation ---- }
     NORM ( rTgtijk,  R );
     CROSS( rTgtijk,vTgtijk, RV );
     NORM ( RV,  W );
     CROSS( W,R, S );

     IJK_RSW( RTgtijk,VTgtijk,R,S,W, TOO, RTgtrsw,VTgtrsw );  { IJK to RSW }

     RTem[1]:= RTgtrsw[1]+RHill[1];  { in RSW }
     RTem[2]:= RTgtrsw[2];
     RTem[3]:= RTgtrsw[3];
     vTem[1]:= vTgtrsw[1]+VHill[1];
     vTem[2]:= vTgtrsw[2]+VHill[2];
     vTem[3]:= vTgtrsw[3]+VHill[3];
     MAG( rTem);
     MAG( vTem);

     { ---- Now perform rotation to fix y ---- }
     IF ABS(RHill[2]) > 0.0000001 THEN
       BEGIN
         angly:= ARCTAN(RHill[2]/rTem[4]); { rtgt, but IF x non-zero, needs extra }
         ROT3( rTem,-angly,  rTem ); { should be ROT3(a) but opp }
         ROT3( vTem,-angly,  vTem );
       END
       ELSE
         angly:= 0.0;

     { ---- Now perform rotation to fix z ---- }
     IF ABS(RHill[3]) > 0.0000001 THEN
       BEGIN
         anglz:= ARCTAN(RHill[3]/rTem[4]); { should be ROT2(-a), but opp }
         ROT2( rTem,anglz,  rTem );
         ROT2( vTem,anglz,  vTem );
       END
       ELSE
         anglz:= 0.0;

     { ---- Now back to IJK via MatMult!! ---- }
     IJK_RSW( RIntijk,VIntijk,R,S,W, FROM, RTem,VTem );  { RSW to IJK }
   END;  { PROCEDURE Hill2Cow }


BEGIN

END.  { Unit AstManv }
{}
