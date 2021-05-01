(*     ----------------------------------------------------------------      *)

                               UNIT ASTUTIL;

(*                                                                           *)
(*  This file contains some utility routines for character operations.       *)
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
(*            30 May 07  David Vallado                                       *)
(*                         3rd edition baseline                              *)
(*  Changes :                                                                *)
(*            28 Jan 04  David Vallado                                       *)
(*                         Update headers                                    *)
(*            14 May 01  David Vallado                                       *)
(*                         2nd edition baseline                              *)
(*            23 Nov 87  David Vallado                                       *)
(*                         Original baseline                                 *)
(*                                                                           *)
(*     ----------------------------------------------------------------      *)

                                  INTERFACE

(*     ----------------------------------------------------------------      *)

{$V-}   { Enable variable string passing }

    TYPE
       Str66  = String[  66 ];
       Str255 = String[ 255 ];

     { ------------------- Misc functions for parsing -------------- }

FUNCTION  GETPART            ( InStr                                 : Str255;
                               LocStart,Leng                         : INTEGER  ) : INTEGER;

FUNCTION  GETPARTL           ( InStr                                 : Str255;
                               LocStart,Leng                         : INTEGER  ) : LONGINT;

FUNCTION  GETPARTR           ( InStr                                 : Str255;
                               LocStart,Leng                         : INTEGER  ) : EXTENDED;

FUNCTION  RmSpcs             ( S                                     : Str255   ) : Str255;

FUNCTION  RmSpcs1            ( S                                     : Str255   ) : Str255;

FUNCTION  UpCaseStr          ( S                                     : Str255   ) : Str255;


(*     ----------------------------------------------------------------      *)

                                IMPLEMENTATION

(*     ----------------------------------------------------------------      *)

{}
{ ------------------------------------------------------------------------------
|
|                           FUNCTION GETPART
|
|  These functions parse a section of a string and return various types of
|    values - Real, INTEGER, Byte, etc., depending on the last letter in the
|    name.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    InStr       - Input String
|    LocStart    - Location to start parsing
|    Leng        - Length of dersired variable
|
|  Outputs       :
|    FUNCTION    - variable of correct type
|
|  Locals        :
|    TempStr     - Temporary string
|    Temp        - Temporary variable
|    Code        - Error variable
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

FUNCTION GETPART             ( InStr                                 : Str255;
                               LocStart,Leng                         : INTEGER ) : INTEGER;
   VAR TempStr   : Str255;
       Temp,Code : INTEGER;
   BEGIN
     { ------------ INTEGER solution -------------- }
     TempStr:= RmSpcs( COPY(InStr,LocStart,Leng) );
     IF POS('D',TempStr) <> 0 THEN
         TempStr[ POS('D',TempStr) ]:= 'E';
     VAL(TempStr,Temp,Code);
     GETPART:= Temp;
   END;  { FUNCTION GETPART }


FUNCTION GETPARTL            ( InStr                                 : Str255;
                               LocStart,Leng                         : INTEGER ) : LONGINT;
   VAR TempStr : Str255;
       Temp    : LONGINT;
       Code    : INTEGER;
   BEGIN
     { ------------ INTEGER solution -------------- }
     TempStr:= RmSpcs( COPY(InStr,LocStart,Leng) );
     VAL(TempStr,Temp,Code);
     GETPARTL:= Temp;
   END;  { FUNCTION GETPARTL }


FUNCTION GETPARTR            ( InStr                                 : Str255;
                               LocStart,Leng                         : INTEGER ) : EXTENDED;
   VAR TempStr : Str255;
       Code    : INTEGER;
       Temp    : EXTENDED;
   BEGIN
     { ------------- Real solution ---------------- }
     TempStr:= RmSpcs( COPY(InStr,LocStart,Leng) );
     IF POS('D',TempStr) <> 0 THEN
         TempStr[ POS('D',TempStr) ]:= 'E';
     VAL(TempStr,Temp,Code);
     GETPARTR:= Temp;
   END;  { FUNCTION GETPARTR }
{}
{ -----------------------------------------------------------------------------
|
|                           FUNCTION RMSPCS
|
|  This Function removes leading and trailing spaces from a string.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs        :
|    S           - String
|
|  Outputs       :
|    RmSPcs      - Result
|
|  Locals        :
|    None.
|
|  Constants     :
|    None.
|
|  Coupling      :
|    None.
|
 ---------------------------------------------------------------------------- }

FUNCTION RmSpcs                 ( S                        : Str255 ) : Str255;
   BEGIN
     While ( S[1] = #$20 ) AND ( Length( S ) > 0 ) Do
       Delete( S, 1, 1 );
     While ( S[ Length( S ) ] = #$20 ) AND ( Length( S ) > 0 ) Do
       Delete( S, Length( S ), 1 );
     RmSpcs := S;
   END;

{ remove spaces and tabs }
FUNCTION RmSpcs1                ( S                        : Str255 ) : Str255;
   BEGIN
     While ( (S[1] = #$20) or (S[1] = #$9) ) AND ( Length( S ) > 0 ) Do
       Delete( S, 1, 1 );
     While ( S[ Length( S ) ] = #$20 ) AND ( Length( S ) > 0 ) Do
       Delete( S, Length( S ), 1 );
     RmSpcs1 := S;
   END;


{ -----------------------------------------------------------------------------
|
|                           FUNCTION UPCASESTR
|
|  This Function converts a string to all UPPERCASE.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs        :
|    S           - String
|
|  Outputs       :
|    UpCaseStr   - Result
|
|  Locals        :
|    i           - Index
|
|  Constants     :
|    None.
|
|  Coupling      :
|    None.
|
 ---------------------------------------------------------------------------- }

FUNCTION UpCaseStr              ( S                        : Str255 ) : Str255;
   Var
       i : Integer;
   BEGIN
     For i := 1 To Length( S ) Do
         S[i] := UpCase( S[i] );
     UpCaseStr := S;
   END;  { Function UpCaseStr }


BEGIN

END.  { Unit AstUtil }
{}
