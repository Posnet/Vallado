(*     ----------------------------------------------------------------      *)
(*                                                                           *)

                               UNIT CONSTS;

(*                                                                           *)
(*  This file contains all of the constants and conversions used for         *)
(*  mathematical procedures.                                                 *)
(*                                                                           *)
(*                           Companion code for                              *)
(*             Fundamentals of Astrodynamics and Applications                *)
(*                                  2007                                     *)
(*                            by David Vallado                               *)
(*                                                                           *)
(*     (W) 719-573-2600, email dvallado@agi.com                              *)
(*                                                                           *)
(*     *****************************************************************     *)
(*                                                                           *)
(*  Current :                                                                *)
(*            28 Jan 04  David Vallado                                       *)
(*                         Update headers                                    *)
(*  Changes :                                                                *)
(*            14 May 01  David Vallado                                       *)
(*                         2nd edition baseline                              *)
(*            23 Nov 87  David Vallado                                       *)
(*                         Original Baseline                                 *)
(*                                                                           *)
(*     ----------------------------------------------------------------      *)

                                  INTERFACE

(*     ----------------------------------------------------------------      *)

    Uses
       AstMath;

  Procedure WGS                 ( WhichOne                 : BYTE );

  Function  GetConvNbr          ( Numbr                    : BYTE ): Extended;

  Procedure ConvertRNum         ( VAR RNum                 : Extended;
                                  Numbr                    : BYTE;
                                  WhichWay                 : CHAR );

    VAR
       RadiusEarthNM,   RadiusEarthFt,  TUMin,       TUSec,       TUDay,
       OmegaEarthr,
       OmegaEarth,      RadiusEarthKm,  VFtPerSec,   VKmPerSec,   EESqrd,
       DegPerSec,       Flat,           Mu,     MuKms,     RadPerDay,   Rad,
       TwoPi,           Pi              : Extended;


(*     ----------------------------------------------------------------      *)

                                IMPLEMENTATION

(*     ----------------------------------------------------------------      *)
{}
{ -----------------------------------------------------------------------------
|
|                           PROCEDURE WGS
|
|  This procedure initializes all the constants and conversions for a
|    program.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs        :
|    WhichOne    - Which WGS to use       2, 96, 84, 72
|
|  OutPuts       :
|    None.
|
|  Locals        :
|    None.
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

PROCEDURE WGS                   ( WhichOne                 : BYTE );
   BEGIN
     IF WhichOne = 2 THEN
       BEGIN            { -- JGM-2, JGM-3, and EGM-96 values  -- }
         RadiusEarthKm  :=   6378.136300000;     { km      }
         Flat           :=   1.0 / 298.257;
         OmegaEarthr    :=      7.2921158553E-5; { rad/sec }
         MuKms          :=      398600.4415;     { km3/s2  }
       END;

     IF WhichOne = 96 THEN
       BEGIN            { -- WGS-84/EGM-96 values -- }
         RadiusEarthKm  :=   6378.137000000;     { km      }
         Flat           :=   1.0 / 298.2572235630;
         OmegaEarthr    :=      7.2921158553E-5; { rad/sec }
         MuKms          :=      398600.4418;     { km3/s2  }
       END;

     IF WhichOne = 84 THEN
       BEGIN            { -- WGS 84 values -- }
         RadiusEarthKm  :=   6378.137000000;     { km      }
         Flat           :=   1.0 / 298.2572235630;
         OmegaEarthr    :=      7.2921158553E-5; { rad/sec }
         MuKms          :=      398600.47;       { km3/s2  }
       END;

     IF WhichOne = 72 THEN
       BEGIN            { -- WGS 72 values -- }
         RadiusEarthKm  :=   6378.135000000;     { km      }
         Flat           :=   1.0 / 298.26;
         OmegaEarthr    :=      7.2921151470E-5; { rad/sec }
         MuKms          :=      398600.5;        { km3/s2  }
       END;

     TwoPi    :=  6.28318530717959;
     Pi       :=  3.14159265358979;
     Rad      := 57.29577951308230;

     RadiusEarthNM  := RadiusEarthKm / 1.852;
     RadiusEarthFt  := RadiusEarthKm*1000.0 / 0.3048;
     TUSec          := SQRT( RadiusEarthKm*RadiusEarthKm*RadiusEarthKm / Mukms );
     TUMin          := TUSec / 60.0;
     TUDay          := TUSec / 86400.0;
     OmegaEarth     := OmegaEarthr * TUSec;
     VKmPerSec      := SQRT( MuKms / RadiusEarthkm );
     VFtPerSec      := VKmPerSec*1000.0 / 0.3048;
     EESqrd         := 2.0*Flat - Flat*Flat;

     DegPerSec:= Rad/TUSec;      { This is actually BACKWARDS }
     { The conversion between deg/sec and rad/tu is opposite, but
       since the menu ONLY / on entry, and * on display, the constant
       is flipped.  Defined as TUSec/Rad, the correct way, the
            NORMAL conversion is 0.5 deg/sec / degpersec = rad/tu }
     RadPerDay:= 1.002737909350795*TwoPi{OmegaEarth / TUDay};
     Mu       := 1.0;            { in DU3/TU2 }

   END;  { Procedure WGS }
{}
{ -----------------------------------------------------------------------------
|
|                           FUNCTION GETCONVNBR
|
|  This function gets the conversion value from the number which is passed in.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs        :
|    Numbr       - Code Number for the particular conversion 1 to 11
|
|  OutPuts       :
|                  The conversion number
|
|  Locals        :
|    None.
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION GetConvNbr             ( Numbr                    : BYTE ): Extended;
   BEGIN
     CASE Numbr OF
       0 : GetConvNbr:= 1.0;  { Leave Unchanged }
       1 : GetConvNbr:= RadiusEarthNM;
       2 : GetConvNbr:= RadiusEarthFt;
       3 : GetConvNbr:= RadiusEarthKm;
       4 : GetConvNbr:= TUMin;
       5 : GetConvNbr:= TUSec;
       6 : GetConvNbr:= Rad;
       7 : GetConvNbr:= OmegaEarth;
       8 : GetConvNbr:= VFtPerSec;
       9 : GetConvNbr:= VKmPerSec;
      10 : GetConvNbr:= EESqrd;
      11 : GetConvNbr:= DegPerSec;
     END;  { Case }
   END;  { Function GetConvNbr }


{ -----------------------------------------------------------------------------
|
|                           PROCEDURE ConvertRNum
|
|  This procedure converts a number using one of the global conversion
|    factors.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs        :
|    RNum        - Number to convert
|    Numbr       - Code Number of the conversion                 1 to 11
|    WhichWay    - Which way to convert, divide or multiply        D M
|
|  OutPuts       :
|    RNum        - The converted number
|
|  Locals        :
|    None.
|
|  Coupling      :
|    GetConvNBR    Gets the conversion factor
|
 ----------------------------------------------------------------------------- }

PROCEDURE ConvertRNum           ( VAR RNum                 : Extended;
                                  Numbr                    : BYTE;
                                  WhichWay                 : CHAR );
   BEGIN
     IF Numbr <> 0 THEN
       BEGIN
         IF WhichWay = 'D' THEN
             RNum:= RNum / GetConvNbr( Numbr )
           ELSE
             RNum:= RNum * GetConvNbr( Numbr );
       END;  { If numbr <> 0 }
   END;  { Procedure ConvertRnum }


BEGIN

END.  { Unit Consts }
{}
