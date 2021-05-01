(*     ----------------------------------------------------------------     *)  

                               UNIT ASTTIME;

(*   This file contains fundamental Astrodynamic procedures and functions
*   relating to the time functions. These routines are discussed in Ch 3
*   and Ch 5.
*
*                            companion code for
*               fundamentals of astrodynamics and applications
*                                   2007
*                             by david vallado
*
*       (w) 719-573-2600, email dvallado@agi.com
*
*    current :
*              15 mar 07  david vallado
*                           3rd edition baseline
*    changes :
*              21 jul 05  david vallado
*                           2nd printing baseline
*              28 Jan 04  David Vallado                                       
*                           Update headers                                    
*              14 May 01  David Vallado                                       
*                           2nd edition baseline                              
*              23 Nov 87  David Vallado                                       
*                           Original baseline                                 
*                                                                           
*      ----------------------------------------------------------------      *)

                                  INTERFACE

(*     ----------------------------------------------------------------      *)
{$V-}
    Uses
        AstMath,
        AstUtil;

    TYPE
        Str12 = STRING[12];
        Str11 = STRING[11]; { not used in this unit }
        Str10 = STRING[10]; { not used in this unit }
        Str3  = STRING[3];
        Direct = (FROM,TOO);
        TimeRec = RECORD
                    Year        : INTEGER;
                    Mon,Day     : BYTE;
                    DUT1        : EXTENDED;
                    DAT         : BYTE;
                    xp,yp       : EXTENDED;
                    XLOD        : EXTENDED;
                    DDPsi,DDEps : EXTENDED;
                  END;


    VAR
        Show        : CHAR;
        FileOut     : TEXT;
        LMonth      : ARRAY[1..12] of INTEGER;
        DayTitle    : ARRAY[1..7]  of Str3;
        MonthTitle  : ARRAY[1..12] of Str3;

{}
FUNCTION  GETINTMON          ( MonStr                                : Str3     ): BYTE;

FUNCTION  GETINTDAY          ( DayStr                                : Str3     ): BYTE;

PROCEDURE INITTIME;

FUNCTION  DAYOFWEEK          ( JD                                    : EXTENDED ): INTEGER;

PROCEDURE DAYLIGHTST         ( Year                                  : INTEGER;
                               Lon                                   : EXTENDED;
                               VAR StartDay, StopDay                 : INTEGER;
                               VAR JDStartDST, JDStopDST             : EXTENDED );


PROCEDURE JDAY               ( Year,Mon,Day,Hr,MIN                   : INTEGER;
                               SEC                                   : EXTENDED;
                               VAR JD                                : EXTENDED );

PROCEDURE JDAYALL            ( Year,Mon,Day,Hr,MIN                   : INTEGER;
                               SEC                                   : EXTENDED;
                               WhichType                             : CHAR;
                               VAR JD                                : EXTENDED );

PROCEDURE DAYS2MDHMS         ( Year                                  : INTEGER;
                               Days                                  : EXTENDED;
                               VAR Mon,Day,Hr,MIN                    : INTEGER;
                               VAR SEC                               : EXTENDED );

PROCEDURE INVJDAY            ( JD                                    : EXTENDED;
                               VAR Year,Mon,Day,Hr,MIN               : INTEGER;
                               VAR SEC                               : EXTENDED );

PROCEDURE FINDDAYS           ( Year,Month,Day,Hr,MIN                 : INTEGER;
                               SEC                                   : EXTENDED;
                               VAR Days                              : EXTENDED );

FUNCTION  GSTIME             ( JDUT1                                 : EXTENDED ): EXTENDED;

FUNCTION  GSTIM0             ( Year                                  : INTEGER  ): EXTENDED;

PROCEDURE LSTIME             ( Lon,JDUT1                             : EXTENDED;
                               VAR LST,GST                           : EXTENDED );

PROCEDURE CONVTIME           ( FileN1                                : Str64;
                               Year,Mon,Day,Hr,MIN                   : INTEGER;
                               SEC                                   : EXTENDED;
                               TimeZone                              : INTEGER;
                               TypeUTIn                              : CHAR;
                               VAR DUT1,DAT,xp,yp,UT1,TUT1,JDUT1,UTC,JDUTC,
                               TAI,TT,TTT,JDTT,TDB,TTDB,JDTDB,DDPsi,DDEps,XLOD : EXTENDED;
                               VAR Error                             : Str12   );

PROCEDURE SUNRISESET         ( JD,Latgd,Lon                          : EXTENDED;
                               WhichKind                             : CHAR;
                               VAR UTSunRise, UTSunSet               : EXTENDED;
                               VAR Error                             : Str12    );

PROCEDURE MOONRISESET        ( JD,Latgd,Lon                          : EXTENDED;
                               VAR UTMoonRise, UTMoonSet,MoonPhaseAng: EXTENDED;
                               VAR Error                             : Str12    );

PROCEDURE HMS_UT             ( VAR Hr,MIN                            : INTEGER;
                               VAR SEC                               : EXTENDED;
                               Direction                             : Direct;
                               VAR UT                                : EXTENDED );

PROCEDURE HMS_SEC            ( VAR Hr,MIN                            : INTEGER;
                               VAR SEC                               : EXTENDED;
                               Direction                             : Direct;
                               VAR UTSec                             : EXTENDED );

PROCEDURE HMS_RAD            ( VAR Hr,MIN                            : INTEGER;
                               VAR SEC                               : EXTENDED;
                               Direction                             : Direct;
                               VAR HMS                               : EXTENDED );

PROCEDURE DMS_RAD            ( VAR Deg,MIN                           : INTEGER;
                               VAR SEC                               : EXTENDED;
                               Direction                             : Direct;
                               VAR DMS                               : EXTENDED );

{ ------------------------- Constants used in this Library ---------------------
|
|     Pi         : EXTENDED =     3.14159265358979;
|     TwoPi      : EXTENDED =     6.28318530717959;
|     Rad2Deg    : EXTENDED =    57.29577951308230;
|     Deg2Rad    : EXTENDED =     0.01745329251994;
|     RadPerDay  : EXTENDED =     6.30038809866574;     Earth Rot 1 solar day
|     OmegaEarth : EXTENDED =     0.05883359221938136;  Earth Rot rad/TU
|
 ----------------------------------------------------------------------------- }

(*     -----------------------------------------------------------------      *)

                                IMPLEMENTATION

(*     -----------------------------------------------------------------      *)
{}
PROCEDURE INITTIME;
   VAR
     i : BYTE;
   BEGIN
     FOR i:= 1 to 12 DO
       BEGIN
         CASE i OF
           1,3,5,7,8,10,12 : LMonth[i]:= 31;
           4,6,9,11        : LMonth[i]:= 30;
           2               : LMonth[i]:= 28;
         END;  { Case }
       END;
     FOR i:= 1 to 12 DO
       BEGIN
         CASE i OF
           1 : MonthTitle[i]:= 'Jan';
           2 : MonthTitle[i]:= 'Feb';
           3 : MonthTitle[i]:= 'Mar';
           4 : MonthTitle[i]:= 'Apr';
           5 : MonthTitle[i]:= 'May';
           6 : MonthTitle[i]:= 'Jun';
           7 : MonthTitle[i]:= 'Jul';
           8 : MonthTitle[i]:= 'Aug';
           9 : MonthTitle[i]:= 'Sep';
          10 : MonthTitle[i]:= 'Oct';
          11 : MonthTitle[i]:= 'Nov';
          12 : MonthTitle[i]:= 'Dec';
         END;  { Case }
       END;
     FOR i:= 1 to 7 DO
       BEGIN
         CASE i OF
           1 : DayTitle[i]:= 'Sun';
           2 : DayTitle[i]:= 'Mon';
           3 : DayTitle[i]:= 'Tue';
           4 : DayTitle[i]:= 'Wed';
           5 : DayTitle[i]:= 'Thr';
           6 : DayTitle[i]:= 'Fri';
           7 : DayTitle[i]:= 'Sat';
         END;  { Case }
       END;
   END;  { PROCEDURE INITTIME }
{}
{ ------------------------------------------------------------------------------
|
|                           FUNCTION GETINTMON / DAY
|
|  this function finds the INTEGER equivalent of the 3 character string
|    representation of month and the day.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    MonStr      - Month name                     'Jan','Feb' ...
|    DayStr      - Day name string                'Sun','Mon' ...
|
|  OutPuts       :
|    GETINTMON   - INTEGER Month equivalent       1 .. 12
|    GETINTDAY   - INTEGER Day equivalent         1 .. 7
|
|  Locals        :
|    i           - Index
|
|  Coupling      :
|    UPCASESTR   - Converts a string to all uppercase characters -local proc
|
 ----------------------------------------------------------------------------- }

FUNCTION GETINTMON           ( MonStr                                : Str3 ): BYTE;
   VAR
     i : BYTE;
   BEGIN
     i:= 1;
     WHILE ( UPCASESTR(MonthTitle[i]) <> UPCASESTR(MonStr) ) and ( i < 12 ) DO
         i:= i+1;
     GETINTMON:= i;
   END;  { FUNCTION GETINTMON }


FUNCTION GETINTDAY           ( DayStr                                : Str3 ): BYTE;
   VAR
     i : BYTE;
   BEGIN
     i:= 1;
     WHILE ( UPCASESTR(DayTitle[i]) <> UPCASESTR(DayStr) ) and ( i < 12 ) DO
         i:= i+1;
     GETINTDAY:= i;
   END;  { FUNCTION GETINTDAY }


{ ------------------------------------------------------------------------------
|
|                           PROCEDURE DAYOFWEEK
|
|  this function finds the day of the week. Integers are used for the days,
|    1 = 'SUN', 2 = 'Mon', ... 7 = 'Sat'.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    JD          - Julian date of interest        days from 4713 BC
|
|  OutPuts       :
|    DAYOFWEEK   - answer                         1 to 7
|
|  Locals        :
|    None.
|
|  Coupling      :
|    None.
|
|  References    :
|    Vallado       2007, 188, Eq 3-39
|
 ----------------------------------------------------------------------------- }

FUNCTION DAYOFWEEK           ( JD                                    : EXTENDED ): INTEGER;
   BEGIN
     { ----- Be sure JD is at 0.0 h on the day of interest ----- }
     JD       := INT(JD+0.5);

     DAYOFWEEK:= TRUNC( JD - 7* TRUNC( (JD+1)/7 ) + 2 );
   END;  { FUNCTION DAYOFWEEK }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE DAYLIGHTST
|
|  this procedure finds the dates for switiching to daylight savings time in
|    a given year. The date is set as the 2nd sunday in march and the first
|    sunday in november. The DST dates are adjusted -10hr to get 0200, -Zone to get
|    the local time zone, and -1.0hr to process the stop because the local time is
|    on DST before a stop.
|
|  Author        : David Vallado                  719-573-2600   17 Mar 2007
|
|  Inputs          Description                    Range / Units
|    Year        - Year                           1900 .. 2100
|    Lon         - SITE longitude (WEST -)        -2Pi to 2Pi rad
|
|  Outputs       :
|    StartDay    - Day in April when DST begins   1 .. 28,29,30,31
|    StopDay     - Day in October when DST ends   1 .. 28,29,30,31
|
|  Locals        :
|    DW          - Day of the week                1 .. 7
|    JDStartDST  - Julian date of start           Days from 4713 BC
|    JDStopDST   - Julian date of stop            Days from 4713 BC
|    Zone        - Time zone of site. Default
|                  of 0.0 gives Greenwich         hrs
|
|  Coupling      :
|    JDAY   - Find the Julian Date
|
|  References    :
|    Vallado       2007, 188
|
 ----------------------------------------------------------------------------- }

PROCEDURE DAYLIGHTST         ( Year                                  : INTEGER;
                               Lon                                   : EXTENDED;
                               VAR StartDay, StopDay                 : INTEGER;
                               VAR JDStartDST, JDStopDST             : EXTENDED );
   CONST
     Rad2Deg    : EXTENDED =    57.29577951308230;
   VAR
     DW   : INTEGER;
     Zone : Extended;
   BEGIN
     { ------- Find time zone information to adjust to a site ------- }
     Zone:= ROUND( Lon*Rad2Deg/15.0 );
     IF Zone > 0 THEN
         Zone:= Zone - 24.0;

     If Year < 2007 then
       begin
         StartDay:= 0;
         REPEAT
             StartDay:= StartDay + 1;
             jday( Year,4,StartDay,12,0,0.0, JDStartDST );
             DW:= TRUNC( JDStartDST - 7* TRUNC( (JDStartDST+1)/7 ) + 2 );
         UNTIL (DW = 1) or (StartDay = 8); { 1 is Sunday }
         JDStartDST:= JDStartDST + (-10.0 - Zone) / 24.0;  { set to 0200 local }

         StopDay:= 32;
         REPEAT
             StopDay:= StopDay - 1;
             jday( Year,10,StopDay,12,0,0.0, JDStopDST );
             DW:= TRUNC( JDStopDST - 7* TRUNC( (JDStopDST+1)/7 ) + 2 );
         UNTIL (DW = 1) or (StopDay = 24); { 1 is Sunday }
         JDStopDST:= JDStopDST + (-10.0 - Zone - 1.0) / 24.0;  { set to 0200 local }
       end
       else
       begin
         StartDay:= 7;
         REPEAT
             StartDay:= StartDay + 1;
             JDAY( Year,3,StartDay,12,0,0.0, JDStartDST );
             DW:= TRUNC( JDStartDST - 7* TRUNC( (JDStartDST+1)/7 ) + 2 );
         UNTIL (DW = 1) or (StartDay = 8); { 1 is Sunday }
         JDStartDST:= JDStartDST + (-10.0 - Zone) / 24.0;  { set to 0200 local }

         StopDay:= 0;
         REPEAT
             StopDay:= StopDay + 1;
             JDAY( Year,11,StopDay,12,0,0.0, JDStopDST );
             DW:= TRUNC( JDStopDST - 7* TRUNC( (JDStopDST+1)/7 ) + 2 );
         UNTIL (DW = 1) or (StopDay = 8); { 1 is Sunday }
         JDStopDST:= JDStopDST + (-10.0 - Zone - 1.0) / 24.0;  { set to 0200 local }
       end;

   END;  { PROCEDURE DAYLIGHTST }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE JDAY
|
|  this procedure finds the Julian date given the Year, Month, Day, and Time.
|    The Julian date is defined by each elapsed day since noon, Jan 1, 4713 BC.
|
|  Algorithm     : Calculate the answer in one step for efficiency
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Year        - Year                           1900 .. 2100
|    Mon         - Month                          1 .. 12
|    Day         - Day                            1 .. 28,29,30,31
|    Hr          - Universal Time Hour            0 .. 23
|    MIN         - Universal Time MIN             0 .. 59
|    SEC         - Universal Time SEC             0.0 .. 59.999
|    WhichType   - Julian or Gregorian calender   'J' or 'G'
|
|  Outputs       :
|    JD          - Julian Date                    days from 4713 BC
|
|  Locals        :
|    B           - Var to aid Gregorian dates
|
|  Coupling      :
|    None.
|
|  References    :
|    Vallado       2007, 189, Alg 14, Ex 3-14
|
 ----------------------------------------------------------------------------- }

PROCEDURE JDAY          ( Year,Mon,Day,Hr,MIN                   : INTEGER;
                               SEC                                   : EXTENDED;
                               VAR JD                                : EXTENDED );
   BEGIN
     JD:= 367.0 * Year
          - TRUNC( (7* (Year+TRUNC ( (Mon+9)/12) ) ) * 0.25 )
          + TRUNC( 275*Mon / 9 )
          + Day + 1721013.5
          + ( (SEC/60.0 + MIN ) / 60.0 + Hr ) / 24.0;  { UT in days }
          { - 0.5*SGN(100.0*Year + Mon - 190002.5) + 0.5; }
   END;  { PROCEDURE JDAY }
{
|    Vallado       2007, 190
}
PROCEDURE JDAYALL       ( Year,Mon,Day,Hr,MIN                   : INTEGER;
                               SEC                                   : EXTENDED;
                               WhichType                             : CHAR;
                               VAR JD                                : EXTENDED );
   VAR
     B : EXTENDED;
   BEGIN
     IF Mon <= 2 THEN
       BEGIN
         Year:= Year - 1;
         Mon := Mon + 12;
       END;
     IF WhichType = 'J' THEN
         { --------- Use for Julian calender, every 4 years --------- }
         B:= 0.0
       ELSE
         { ---------------------- Use for Gregorian ----------------- }
         B:= 2 - TRUNC(Year*0.01) + TRUNC(TRUNC(Year*0.01)*0.25);
     JD:= TRUNC( 365.25*(Year + 4716) )
          + TRUNC( 30.6001*(Mon+1) )
          + Day + B - 1524.5
          + ( (SEC/60.0 + MIN ) / 60.0 + Hr ) / 24.0;  { UT in days }
   END;  { PROCEDURE JDAYALL }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE DAYS2MDHMS
|
|  this procedure converts the day of the year, days, to the equivalent month
|    day, hour, Minute and second.
|
|  Algorithm     : Set up array for the Number of days per month
|                  Find Leap Year - Use 1900 because 2000 is a leap year
|                  Loop through a Temp value while the value is < the days
|                  Perform INTEGER conversions to the correct day and month
|                  Convert remainder into H M S using type conversions
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Year        - Year                           1900 .. 2100
|    Days        - Julian Day of the year         0.0  .. 366.0
|
|  OutPuts       :
|    Mon         - Month                          1 .. 12
|    Day         - Day                            1 .. 28,29,30,31
|    Hr          - Hour                           0 .. 23
|    MIN         - Minute                         0 .. 59
|    SEC         - Second                         0.0 .. 59.999
|
|  Locals        :
|    DayofYr     - Day of year
|    Temp        - Temporary EXTENDED values
|    IntTemp     - Temporary INTEGER value
|    i           - Index
|    LMonth[12]  - INTEGER Array containing the Number of days per month
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

PROCEDURE DAYS2MDHMS         ( Year                                  : INTEGER;
                               Days                                  : EXTENDED;
                               VAR Mon,Day,Hr,MIN                    : INTEGER;
                               VAR SEC                               : EXTENDED );
   VAR
     IntTemp, i, DayofYr : INTEGER;
     Temp                : EXTENDED;
     LMonth              : Array[1..12] of INTEGER;
   BEGIN
     { --------------- Set up array of days in month  --------------- }
     FOR i:= 1 to 12 DO
       BEGIN
         CASE i OF
           1,3,5,7,8,10,12 : LMonth[i]:= 31;
           4,6,9,11        : LMonth[i]:= 30;
           2               : LMonth[i]:= 28;
         END;  { Case }
       END;

     DayofYr:= TRUNC(Days );

     { ----------------- Find month and Day of month ---------------- }
     IF ( (Year-1900) MOD 4 ) = 0 THEN
         LMonth[2]:= 29;
     i:= 1;
     IntTemp:= 0;
     WHILE ( DayofYr > IntTemp + LMonth[i] ) and ( i < 12 ) DO
       BEGIN
         IntTemp:= IntTemp + LMonth[i];
         i:= i+1;
       END;
     Mon:= i;
     Day:= DayofYr - IntTemp + 1;

     { ----------------- Find hours minutes and seconds ------------- }
     Temp:= (Days - DayofYr )*24.0;
     Hr  := TRUNC( Temp );
     Temp:= (Temp-Hr) * 60.0;
     MIN := TRUNC( Temp );
     SEC := (Temp-MIN) * 60.0;

        { ---- Check for roundoff errors }
        IF (Sec >= 59.9999) THEN
          begin
            Sec := 0.0;
            Min := Min + 1;
            IF (Min > 59) THEN
              begin
                Min := 0;
                Hr := Hr + 1;
                IF (Hr > 23) THEN
                  begin
                    Hr := 0;
                    Day := Day + 1;
                  END;
              END;
          END;
   END;  { PROCEDURE DAYS2MDHMS }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE INVJDAY
|
|  this procedure finds the Year, month, day, hour, Minute and second
|  given the Julian date. TU can be UT1, TDT, TDB, etc.
|
|  Algorithm     : Set up starting values
|                  Find Leap Year - Use 1900 because 2000 is a leap year
|                  Find the elapsed days through the year in a loop
|                  Call routine to find each individual value
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    JD          - Julian Date                    days from 4713 BC
|
|  OutPuts       :
|    Year        - Year                           1900 .. 2100
|    Mon         - Month                          1 .. 12
|    Day         - Day                            1 .. 28,29,30,31
|    Hr          - Hour                           0 .. 23
|    MIN         - Minute                         0 .. 59
|    SEC         - Second                         0.0 .. 59.999
|
|  Locals        :
|    Days        - Day of year plus fractional
|                  portion of a day               days
|    Tu          - Julian Centuries from 0 h
|                  Jan 0, 1900
|    Temp        - Temporary real values
|    LeapYrs     - Number of Leap years from 1900
|
|  Coupling      :
|    DAYS2MDHMS  - Finds Month, day, hour, Minute and second given Days and Year
|
|  References    :
|    Vallado       2007, 208, Alg 22, Ex 3-13
|
 ----------------------------------------------------------------------------- }

PROCEDURE INVJDAY       ( JD                                    : EXTENDED;
                               VAR Year,Mon,Day,Hr,MIN               : INTEGER;
                               VAR SEC                               : EXTENDED );
   VAR
     LeapYrs         : INTEGER;
     Days, Tu, Temp  : EXTENDED;
   BEGIN
     { --------------- Find Year and Days of the year --------------- }
     Temp   := JD-2415019.5;
     Tu     := Temp / 365.25;
     Year   := 1900 + TRUNC( Tu );
     LeapYrs:= TRUNC( ( Year-1901 )*0.25 );
     Days   := Temp - ((Year-1900)*365.0 + LeapYrs ){ + 0.00000000001}; { Nudge by 8.64x10-7 Sec to get even outputs }

     { ------------ Check for case of beginning of a year ----------- }
     IF Days < 1.0 THEN
       BEGIN
         Year   := Year - 1;
         LeapYrs:= TRUNC( ( Year-1901 )*0.25 );
         Days   := Temp - ((Year-1900)*365.0 + LeapYrs );
       END;

     { ----------------- Find remaing data  ------------------------- }
     DAYS2MDHMS( Year,Days, Mon,Day,Hr,MIN,SEC );
     Sec:= Sec{ - 0.00000086400};
   END;  { PROCEDURE InverseJDAY }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE FINDDAYS
|
|  this procedure finds the fractional days through a year given the year,
|    month, day, hour, Minute and second.
|
|  Algorithm     : Set up array for the Number of days per month
|                  Find Leap Year - Use 1900 because 2000 is a leap year
|                  Check for a leap year
|                  Loop to find the elapsed days in the year
|
|  Author        : David Vallado                  719-573-2600
|
|  Inputs          Description                    Range / Units
|    Year        - Year                           1900 .. 2100
|    Mon         - Month                          1 .. 12
|    Day         - Day                            1 .. 28,29,30,31
|    Hr          - Hour                           0 .. 23
|    MIN         - Minute                         0 .. 59
|    SEC         - Second                         0.0 .. 59.999
|
|  OutPuts       :
|    Days        - Day of year plus fraction of a
|                    day                          days
|
|  Locals        :
|    LMonth      - Length of months of year
|    i           - Index
|
|  Coupling      :
|    None.
|
|  References    :
|    Vallado       2007, 207, Ex 3-12
|
 ----------------------------------------------------------------------------- }

PROCEDURE FINDDAYS           ( Year,Month,Day,Hr,MIN                 : INTEGER;
                               SEC                                   : EXTENDED;
                               VAR Days                              : EXTENDED );
   VAR
     i      : BYTE;
     LMonth : ARRAY[1..12] of INTEGER;
   BEGIN
     FOR i:= 1 to 12 DO
       BEGIN
         CASE i OF
           1,3,5,7,8,10,12 : LMonth[i]:= 31;
           4,6,9,11 : LMonth[i]:= 30;
           2 : LMonth[i]:= 28;
         END;  { Case }
       END;
     IF (Year-1900) MOD 4 = 0 THEN
         LMonth[2]:= 29;

     i   := 1;
     Days:= 0.0;
     WHILE (i < Month) and ( i < 12 ) DO
       BEGIN
         Days:= Days + LMonth[i];
         i:= i + 1;
       END;

     Days:= Days + Day + Hr/24.0 + MIN/1440.0 + SEC/86400.0;
   END;  { PROCEDURE FINDDAYS }
{}
{ ------------------------------------------------------------------------------
|
|                           FUNCTION GSTIME
|
|  this function finds the Greenwich sidereal time (iau-82).
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    JDUT1       - Julian Date in UT1             days from 4713 BC
|
|  OutPuts       :
|    GSTIME      - Greenwich SIDEREAL Time        0 to 2Pi rad
|
|  Locals        :
|    Temp        - Temporary variable for reals   rad
|    TUT1        - Julian Centuries from the
|                  Jan 1, 2000 12 h epoch (UT1)
|
|  Coupling      :
|    MODFUNC     - MOD FUNCTION for REAL variables
|
|  References    :
|    Vallado       2007, 193, Eq 3-43
|
 ----------------------------------------------------------------------------- }

FUNCTION GSTIME              ( JDUT1                                 : EXTENDED ): EXTENDED;
   CONST
     TwoPi      : EXTENDED =     2.0*Pi;  { 6.28318530717959; }
     Deg2Rad    : EXTENDED =     Pi/180.0;
   VAR
     Temp, TUT1 : EXTENDED;
   BEGIN
     TUT1:= ( JDUT1 - 2451545.0 ) / 36525.0;
     Temp:= - 6.2E-6*TUT1*TUT1*TUT1
            + 0.093104*TUT1*TUT1
            + (876600.0*3600 + 8640184.812866)*TUT1
            + 67310.54841;  {sec }
     Temp:= MODFUNC( Temp*Deg2Rad/240.0,TwoPi ); { 360/86400 = 1/240, to deg, to rad }

     { ------------------------ Check quadrants --------------------- }
     IF Temp < 0.0 THEN
         Temp:= Temp + TwoPi;

     GSTIME:= Temp;

     IF Show = 'Y' THEN
         WriteLn( tut1:18:12 );
   END; { FUNCTION GSTIME }

{ ------------------------------------------------------------------------------
|
|                           FUNCTION GSTIM0
|
|  this function finds the Greenwich sidereal time (iau-82) at the beginning of a year.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Year        - Year                           1998, 1999, etc.
|
|  OutPuts       :
|    GSTIM0      - Greenwich SIDEREAL Time        0 to 2Pi rad
|
|  Locals        :
|    JDUT1       - Julian Date in UT1             days from 4713 BC
|    Temp        - Temporary variable for Reals   rad
|    TUT1        - Julian Centuries from the
|                  Jan 1, 2000 12 h epoch (UT1)
|
|  Coupling      :
|    MODFUNC       MOD FUNCTION for Real variables
|
|  References    :
|    Vallado       2007, 195, Eq 3-46
|
 ----------------------------------------------------------------------------- }

FUNCTION GSTIM0              ( Year                                  : INTEGER ): EXTENDED;
   CONST
     TwoPi      : EXTENDED =     2.0*Pi;  { 6.28318530717959; }
   VAR
     JDUT1, Temp, TUT1 : EXTENDED;
   BEGIN
     JDUT1  := 367.0 * Year - ( INT((7*(Year+TRUNC(10/12)))*0.25) ) +
                             ( TRUNC(275/9) ) + 1721014.5;
     TUT1:= ( JDUT1 - 2451545.0 ) / 36525.0;

     Temp:= 1.75336855923327
            + 628.331970688841*TUT1
            + 6.77071394490334E-06*TUT1*TUT1
            - 4.50876723431868E-10*TUT1*TUT1*TUT1;

     { ------------------------ Check quadrants --------------------- }
     Temp:= MODFUNC( Temp,TwoPi );
     IF Temp < 0.0 THEN
         Temp:= Temp + TwoPi;

     GSTIM0:= Temp;

   END; { FUNCTION GSTIM0 }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE LSTIME
|
|  this procedure finds the Local sidereal time at a given location.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Lon         - SITE longitude (WEST -)        -2Pi to 2Pi rad
|    JDUT1       - Julian Date in UT1             days from 4713 BC
|
|  OutPuts       :
|    LST         - Local sidereal Time            0.0 to 2Pi rad
|    GST         - Greenwich sidereal Time        0.0 to 2Pi rad
|
|  Locals        :
|    None.
|
|  Coupling      :
|    MODFUNC       MOD FUNCTION for REAL variables
|    GSTIME        Finds the Greenwich sidereal Time
|
|  References    :
|    Vallado       2007, 194, alg 15, ex 3-5
|
 ----------------------------------------------------------------------------- }

PROCEDURE LSTIME             ( Lon,JDUT1                             : EXTENDED;
                               VAR LST,GST                           : EXTENDED );
   CONST
     TwoPi      : EXTENDED =     2.0*Pi;  { 6.28318530717959; }
   BEGIN
     GST := GSTIME( JDUT1 );
     LST := Lon + GST;

     { ------------------------ Check quadrants --------------------- }
     LST := MODFUNC( LST,TwoPi );
     IF LST < 0.0 THEN
         LST:= LST + TwoPi;

   END; { PROCEDURE LSTIME }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE CONVTIME
|
|  This PROCEDURE finds the time parameters and Julian century values for inputs
|    of UTC or UT1. Numerous outputs are found as shown in the local variables.
|    Because calucations are in UTC, you must include TimeZone IF you enter a
|    local time, otherwise it should be zero.
|
|  Algorithm     : A file of record contains the timing data
|                  Seeks are performed to obtain the data
|                    Data starts May 19, 1976, thus JD = 2442917.5 in the code
|                  Calculate the answer depending on initial time type
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Year        - Year                           1900 .. 2100
|    Mon         - Month                          1 .. 12
|    Day         - Day                            1 .. 28,29,30,31
|    Hr          - Universal Time Hour            0 .. 23
|    MIN         - Universal Time MIN             0 .. 59
|    SEC         - Universal Time SEC             0.0 .. 59.999
|    TimeZone    - Offset to UTC from local SITE  0 .. 23 hr
|    TypeUTIn    - Type of iinput UT              1 (UT1), else UTC
|
|  Outputs       :
|    DUT1        - Delta of UTC - UT1             SEC
|    DAT         - Delta of UTC - TAI             SEC
|    xp          - Polar motion coefficient       arcsec
|    yp          - Polar motion coefficient       arcsec
|    UT1         - Universal time                 SEC
|    TUT1        - Julian centuries of UT1
|    JDUT1       - Julian Date of UT1             days from 4713 BC
|    UTC         - Coordinated Universal Time     SEC
|    JDUTC       - Julian Date of UTC             days from 4713 BC
|    TAI         - Atomic time                    SEC
|    TT          - Terrestrial Dynamical time     SEC
|    TTT         - Julian centuries of TT
|    JDTT        - Julian Date of TT             days from 4713 BC
|    TDB         - Terrestrial Barycentric time   SEC
|    TTDB        - Julian centuries of TDB
|    JDTDB       - Julian Date of TDB             days from 4713 BC
|    Error       - Error flag for PROCEDURE       'ok'
|
|  Locals        :
|    HrTemp      - Temporary hours                hr
|    MinTemp     - Temporary Minutes              MIN
|    SecTemp     - Temporary seconds              SEC
|    LocalHr     - Difference to local time       hr
|    JD          - Julian Date of request         days from 4713 BC
|    ME          - Mean Anomaly of the Earth      rad
|    TimeFile    - File of record with time data
|    CurrTimeRec - Current Time record
|
|  Coupling      :
|    HMS_SEC     - Conversion between hr-MIN-SEC and seconds
|    JDAY   - Find the Julian date
|    MODFUNC     - MOD FUNCTION for Real variables
|
|  References    :
|    vallado       2007, 201, alg 16, ex 3-7
|
 ----------------------------------------------------------------------------- }
{}
PROCEDURE CONVTIME           ( FileN1                                : Str64;
                               Year,Mon,Day,Hr,MIN                   : INTEGER;
                               SEC                                   : EXTENDED;
                               TimeZone                              : INTEGER;
                               TypeUTIn                              : CHAR;
                               VAR DUT1,DAT,xp,yp,UT1,TUT1,JDUT1,UTC,JDUTC,
                               TAI,TT,TTT,JDTT,TDB,TTDB,JDTDB,DDPsi,DDEps,XLOD : EXTENDED;
                               VAR Error                             : Str12   );
   CONST
     Deg2Rad    : EXTENDED =     0.01745329251994;
   VAR
     CurrTimeRec                : TimeRec;
     TimeFile                   : File of TimeRec;
     hrTemp, minTemp, LocalHr   : INTEGER;
     Ratio, Rad,secTemp, ME, Temp, JD  : EXTENDED;
   BEGIN
     Error:= 'ok';
     Rad  := 180.0/Pi;

     ASSIGN( TimeFile,FileN1 );
     RESET( TimeFile);
     JDAY( Year,Mon,Day,0,0,0.0, JD );

     Ratio:= (Hr+Min/60.0+Sec/3600.0) / 24.0;
     IF (TRUNC(JD-2442917.5) > FileSize(TimeFile)) or (JD < 2442917.5) THEN
         Error:= 'DateOutRng'
       ELSE
         SEEK( TimeFile, TRUNC(JD-2442917.5) );

     IF Error = 'ok' THEN
       BEGIN
         Read(TimeFile,CurrTimeRec );
         DUT1 := CurrTimeRec.DUT1; { sec }
         DAT  := CurrTimeRec.DAT;
         xp   := CurrTimeRec.xp;   { " }
         yp   := CurrTimeRec.yp;
         DDPsi:= CurrTimeRec.DDPsi; { " }
         DDEps:= CurrTimeRec.DDEps;
         XLOD := CurrTimeRec.XLOD;

         IF Show = 'I' THEN
           BEGIN
             WriteLn( FileOut,'--------------- Before Interpolation' );
             WriteLn( FileOut,'Time is input in UT',TypeUTIn );
             WriteLn( FileOut,'DUT1  ',DUT1:14:9,'s' );
             WriteLn( FileOut,'DAT   ',DAT:14:9 );
             WriteLn( FileOut,'xp    ',xp:14:9,'"' );
             WriteLn( FileOut,'yp    ',yp:14:9 );
             WriteLn( FileOut,'DDPsi ',DDPsi:14:9,'"' );
             WriteLn( FileOut,'DDEps ',DDEps:14:9 );
             WriteLn( FileOut,'XLOD  ',XLOD:14:9 );
           END;

         { -- Do simple linear interpolation with next day to find values -- }
         WriteLn ( FileOut,' NO INTERPOLATION' );
{
         Read(TimeFile,CurrTimeRec );
         DUT1 := DUT1 + Ratio*(CurrTimeRec.DUT1 - DUT1);
         DAT  := DAT  + Ratio*(CurrTimeRec.DAT  - DAT);
         xp   := xp   + Ratio*(CurrTimeRec.xp   - xp);
         yp   := yp   + Ratio*(CurrTimeRec.yp   - yp);
         DDPsi:= DDPsi+ Ratio*(CurrTimeRec.DDPsi - DDPsi);
         DDEps:= DDEps+ Ratio*(CurrTimeRec.DDEps - DDEps);
         XLOD := XLOD + Ratio*(CurrTimeRec.XLOD - XLOD);
}
         xp   := xp / (3600.0*Rad); { " to rad }
         yp   := yp / (3600.0*Rad);
       END   { IF error }
       ELSE
       BEGIN
         { ---- Set to zero, but still calculate ---- }
         DUT1 := 0.0; DAT  := 0.0;
         xp   := 0.0; yp   := 0.0;
         DDPsi:= 0.0; DDEps:= 0.0;
         XLOD := 0.0;
         WriteLn( FileOut,Error,JD:18:12,' too far' );
       END;
     CLOSE( TimeFile );

     { -------------------- Start IF UT1 is known ------------------- }
     LocalHr:= TimeZone + Hr;
     IF TypeUTIn = '1' THEN
       BEGIN
         HMS_SEC( LocalHr,MIN,SEC, TOO,  UT1 );
         JDAY( Year,Mon,Day, LocalHr, MIN, SEC, JDUT1 );
         TUT1:= (JDUT1 - 2451545.0 )/ 36525.0 ;

         UTC:= UT1 - dUT1;
         HMS_SEC( HrTemp,MinTemp,SecTemp,FROM, UTC );
         JDAY( Year,Mon,Day, HrTemp, MinTemp, SecTemp, JDUTC );
       END
       ELSE
       { ------------------ Start IF UTC is known ------------------- }
       BEGIN
         HMS_SEC( LocalHr,MIN,SEC,TOO,  UTC );
         JDAY( Year,Mon,Day, LocalHr, MIN, SEC, JDUTC );

         UT1:= UTC + DUT1;
         HMS_SEC( HrTemp,MinTemp,SecTemp,FROM, UT1 );
         JDAY( Year,Mon,Day, HrTemp, MinTemp, SecTemp, JDUT1 );
         TUT1:= (JDUT1 - 2451545.0 )/ 36525.0 ;
       END;

     TAI:= UTC + DAT;

     TT:= TAI + 32.184; { SEC }
     HMS_SEC( HrTemp,MinTemp,SecTemp,FROM, TT );
     JDAY( Year,Mon,Day, HrTemp, MinTemp, SecTemp, JDTT );
     TTT:= (JDTT - 2451545.0 )/ 36525.0 ;

     ME:= 357.5277233 + 35999.05034*TTT;  { approx - should do with TTDB }
     ME:= MODFUNC( ME,360.0 );
     ME:= ME * Deg2Rad;
     TDB:= TT + 0.001658 * SIN(ME) + 0.00001385*SIN(2.0*ME);
     HMS_SEC( HrTemp,MinTemp,SecTemp,FROM, TDB );
     JDAY( Year,Mon,Day, HrTemp, MinTemp, SecTemp, JDTDB );
     TTDB:= (JDTDB - 2451545.0 )/ 36525.0 ;

     IF Show = 'I' THEN
       BEGIN
         WriteLn( FileOut,'------ CONVTIME ----------' );
         WriteLn( FileOut,'Input vars --------- ');
         WriteLn( FileOut,'Year    ',Year:4 );
         WriteLn( FileOut,'Month   ',Mon:4 );
         WriteLn( FileOut,'Day     ',Day:4 );
         WriteLn( FileOut,'Hr      ',Hr:4 );
         WriteLn( FileOut,'Min     ',Min:4 );
         WriteLn( FileOut,'Sec     ',Sec:14:12 );
         WriteLn( FileOut,'Time is input in UT',TypeUTIn );
         WriteLn( FileOut,'Intermediate vars -- ');
         WriteLn( FileOut,'Output vars -------- ');
         WriteLn( FileOut,'DUT1  ',DUT1:14:9,'s' );
         WriteLn( FileOut,'DAT   ',DAT:14:9 );
         WriteLn( FileOut,'xp    ',xp*3600.0*Rad:14:9,'"' );
         WriteLn( FileOut,'yp    ',yp*3600.0*Rad:14:9 );
         WriteLn( FileOut,'DDPsi ',DDPsi:14:9,'"' );
         WriteLn( FileOut,'DDEps ',DDEps:14:9 );
         WriteLn( FileOut,'XLOD  ',XLOD:14:9 );
         WriteLn( FileOut,'UT1   ',UT1:14:9,'s' );
         WriteLn( FileOut,'UTC   ',UTC:14:9,'s' );
         WriteLn( FileOut,'TAI   ',TAI:14:9,'s' );
         WriteLn( FileOut,'TT    ',TT:14:9,'s' );
         WriteLn( FileOut,'TDB   ',TDB:14:9,'s' );
         WriteLn( FileOut,'TUT1  ',TUT1:14:9,'s' );
         WriteLn( FileOut,'TTT   ',TTT:14:9,'s' );
         WriteLn( FileOut,'TTDB  ',TTDB:14:9,'s' );
         WriteLn( FileOut,'JDUT1 ',JDUT1:18:12,'s' );
         WriteLn( FileOut,'JDUTC ',JDUTC:18:12,'s' );
         WriteLn( FileOut,'JDTT  ',JDTT:18:12,'s' );
         WriteLn( FileOut,'JDTDB ',JDTDB:18:12,'s' );
       END;
   END;  { PROCEDURE CONVTIME }

{ ------------------------------------------------------------------------------
|
|                           PROCEDURE SUNRISESET
|
|  this procedure finds the Universal time for Sunrise and Sunset given the
|    day and SITE location.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    JD          - Julian Date                    days from 4713 BC
|    Latgd       - SITE latitude (SOUTH -)        -65ø to 65ø rad
|    Lon         - SITE longitude (WEST -)        -2Pi to 2Pi rad
|    WhichKind   - Character for which rise/set   'S' 'C' 'N' 'A'
|
|  OutPuts       :
|    UTSunRise   - Universal time of sunrise      hrs
|    UTSunSet    - Universal time of sunset       hrs
|    Error       - Error Parameter
|
|  Locals        :
|    SunAngle    - ANGLE between the SUN vector
|                  and a point on the Earth       rad
|    JDTemp      - Julian date for sunrise/set    days from 4713 BC
|    UTTemp      - Temporary UT time              days
|    TUT1        - Julian Centuries from the
|                  Jan 1, 2000 12 h epoch (UT1)
|    Ra          - Right ascension                rad
|    Decl        - Declination                    rad
|    MeanLonSun  -                                rad
|    MeanAnomalySun                               rad
|    LonEcliptic - Longitude of the ecliptic      rad
|    Obliquity   - Obliquity of the ecliptic      rad
|    GST         - for 0 h UTC of each day        rad
|    LHA         - Local hour ANGLE               rad
|    Year        - Year                           1900 .. 2100
|    Mon         - Month                          1 .. 12
|    Day         - Day                            1 .. 28,29,30,31
|    Hr          - Hour                           0 .. 23
|    MIN         - Minute                         0 .. 59
|    SEC         - Second                         0.0 .. 59.999
|    Opt         - Idx to do rise and set calc    1,2
|
|  Coupling      :
|    INVJDAY- Finds the Year day mon hr MIN SEC from the Julian Date
|    JDAY   - Finds the Julian date given Year, mon day, hr, MIN, SEC
|    ASIN      - Arc sine FUNCTION
|    ACOS      - Arc cosine FUNCTION
|    MODFUNC     - MOD FUNCTION for REAL variables
|
|  References    :
|    Vallado       2007, 283, Alg 30, Ex 5-2
|
 ----------------------------------------------------------------------------- }

PROCEDURE SUNRISESET         ( JD,Latgd,Lon                          : EXTENDED;
                               WhichKind                             : CHAR;
                               VAR UTSunRise, UTSunSet               : EXTENDED;
                               VAR Error                             : Str12 );
   CONST
     TwoPi      : EXTENDED =     2.0*Pi;  { 6.28318530717959; }
     Rad2Deg    : EXTENDED =    180.0 / pi; { 57.29577951308230; }
     Deg2Rad    : EXTENDED =     pi / 180.0; { 0.01745329251994; }
   VAR
     Opt, Year, Month, Day, Hr, MIN : INTEGER;
     JDTemp, UTTemp, SunAngle, TUT1, Ra, SEC, MeanLonSun, MeanAnomalySun,
     LonEcliptic, Decl, Obliquity, GST, LHA     : EXTENDED;
   BEGIN
     Error:= 'ok';
     { --------------- Make sure lon is within +- 180 deg ----------- }
     IF Lon > Pi THEN
         Lon:= Lon - 2.0*Pi;
     IF Lon < -Pi THEN
         Lon:= Lon + 2.0*Pi;

     CASE WhichKind OF
       'S' : SunAngle:= (90.0+50.0/60.0 )*Deg2Rad; { Sunrise / set }
       'C' : SunAngle:=  96.0 *Deg2Rad;            { Civil         }
       'N' : SunAngle:= 102.0 *Deg2Rad;            { Nautical      }
       'A' : SunAngle:= 108.0 *Deg2Rad;            { Astronomical  }
     END;  { Case }
     INVJDAY( JD,  Year,Month,Day,Hr,MIN,SEC );

     FOR Opt:= 1 to 2 DO
       BEGIN
         IF Opt = 1 THEN
             JDAY( Year,Month,Day, 6,0,0.0, JDTemp )  { Sunrise }
           ELSE
             JDAY( Year,Month,Day,18,0,0.0, JDTemp ); { Sunset  }
         JDTemp:= JDTemp - Lon*Rad2Deg/15.0/24.0;

         TUT1 := (JDTemp - 2451545.0)/36525.0;
         MeanLonSun := 280.4606184 + 36000.77005361*TUT1;  { deg }

         MeanAnomalySun:= 357.5277233+35999.05034*TUT1; { deg }
         MeanAnomalySun:= MODFUNC( MeanAnomalySun*Deg2Rad,TwoPi );
         IF MeanAnomalySun < 0.0 THEN
             MeanAnomalySun:= MeanAnomalySun + TwoPi;

         LonEcliptic:= MeanLonSun + 1.914666471*SIN(MeanAnomalySun)
                       + 0.019994643*SIN(2.0*MeanAnomalySun); { deg }
         LonEcliptic:= MODFUNC( LonEcliptic*Deg2Rad,TwoPi );
         IF LonEcliptic < 0.0 THEN
             LonEcliptic:= LonEcliptic + TwoPi;

         Obliquity:= 23.439291 - 0.0130042*TUT1; { deg }
         Obliquity:= Obliquity *Deg2Rad;

         Ra  := ARCTAN( COS(Obliquity) * TAN(LonEcliptic) );
         Decl:= ASIN( SIN(Obliquity) * SIN(LonEcliptic) );

         IF Ra < 0.0 THEN
             Ra:= Ra + TwoPi;
         IF (LonEcliptic > Pi) and (Ra < Pi) THEN
             Ra:= Ra + Pi;
         IF (LonEcliptic < Pi) and (Ra > Pi) THEN
             Ra:= Ra - Pi;

         LHA:= (COS(SunAngle) - SIN(Decl)*SIN(Latgd)) / (COS(Decl)*COS(Latgd) ); {rad}
         IF ABS(LHA) <= 1.0 THEN
             LHA:= ACOS( LHA ) {rad}
           ELSE
             Error:= 'Not ok';
         IF Error = 'ok' THEN
           BEGIN
             IF Opt = 1 THEN
                 LHA:= TwoPi - LHA;  { Sunrise only }
             GST:= 1.75336855923327 + 628.331970688841*TUT1
                   + 6.77071394490334E-06*TUT1*TUT1
                   - 4.50876723431868E-10*TUT1*TUT1*TUT1;
             GST:= MODFUNC( GST,TwoPi );
             IF GST < 0.0 THEN
                 GST:= GST + TwoPi;
             UTTemp:= LHA + Ra  - GST;
             UTTemp:= UTTemp * Rad2Deg/15.0;  { hrs }
             UTTemp:= MODFUNC( UTTemp,24.0 );
             UTTemp:= UTTemp - Lon*Rad2Deg/15.0; { hrs }
             IF UTTemp < 0.0 THEN
               BEGIN
                 UTTemp:= UTTemp + 24.0;
                 Error:= 'Day before';
               END;
             IF UTTemp > 24.0 THEN
               BEGIN
                 UTTemp:= UTTemp - 24.0;
                 Error:= 'Day After';
               END;
           END
           ELSE
             UTTemp:= 99.99;

         IF Opt = 1 THEN
             UTSunRise:= UTTemp
           ELSE
             UTSunSet := UTTemp;
       END;  { For Opt }

   END;  { PROCEDURE SUNRISESET }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE MOONRISESET
|
|  this procedure finds the Universal time for Moonrise and Moonset given the
|    day and SITE location.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    JD          - Julian Date                    days from 4713 BC
|    Latgd       - SITE latitude (SOUTH -)        -65ø to 65ø rad
|    Lon         - SITE longitude (WEST -)        -2Pi to 2Pi rad
|
|  OutPuts       :
|    UTMoonRise  - Universal time of Moonrise     hrs
|    UTMoonSet   - Universal time of Moonset      hrs
|    MoonPhaseAng- Phase angle of the Moon        deg
|    Error       - Error Parameter
|
|  Locals        :
|    MoonAngle   - ANGLE between the Moon vector
|                  and a point on the Earth       rad
|    JDTemp      - Julian date for Moonrise/set   days from 4713 BC
|    UTTemp      - Temporary UT time              days
|    TUT1        - Julian Centuries from the
|                  Jan 1, 2000 12 h epoch (UT1)
|    RtAsc       - Right ascension                rad
|    Decl        - Declination                    rad
|    MeanLonMoon -                                rad
|    MeanAnomaly -                                rad
|    EclpLong    - Longitude of the ecliptic      rad
|    Obliquity   - Obliquity of the ecliptic      rad
|    Try
|    l, m, n     - Direction cosines
|    EclpLat
|    MoonGHA, MoonGHAn
|    DGHA
|    LHAn
|    LST
|    DeltaUT
|    t, tn
|    LonEclSun
|    LonEclMoon
|    TTDB
|    x
|    GST         - for 0 h UTC of each day        rad
|    LHA         - Local hour ANGLE               rad
|    Year        - Year                           1900 .. 2100
|    Mon         - Month                          1 .. 12
|    Day         - Day                            1 .. 28,29,30,31
|    Hr          - Hour                           0 .. 23
|    MIN         - Minute                         0 .. 59
|    SEC         - Second                         0.0 .. 59.999
|    Opt         - Idx to do rise and set calc    1,2
|
|  Coupling      :
|    INVJDAY- Finds the Year day mon hr MIN SEC from the Julian Date
|    JDAY   - Finds the Julian date given Year, mon day, hr, MIN, SEC
|    ASIN      - Arc sine FUNCTION
|    ACOS      - Arc cosine FUNCTION
|    MODFUNC     - MOD FUNCTION for REAL variables
|
|  References    :
|    Vallado       2007, 292, Alg 32, Ex 5-4
|
 ----------------------------------------------------------------------------- }

PROCEDURE MOONRISESET        ( JD,Latgd,Lon                          : EXTENDED;
                               VAR UTMoonRise, UTMoonSet,MoonPhaseAng: EXTENDED;
                               VAR Error                             : Str12 );
   CONST
     TwoPi      : EXTENDED =     2.0*Pi;  { 6.28318530717959; }
     Deg2Rad    : EXTENDED =     pi / 180.0; { 0.01745329251994; }
   VAR
     Try : BYTE;
     l,m,n,EclpLong,EclpLat,Obliquity,MoonGHA,DGHA,LHAn,LHA,LST,MoonGHAn,
     DeltaUT,tn,GST,t,
     LonEclSun,LonEclMoon,MeanAnomaly,MeanLong,ttdb,x,
     sec,JDTemp, UTTemp, RtAsc, Decl : EXTENDED;
     Opt, i, Year, Month, Day, Hr, Min : INTEGER;
   BEGIN
     Error:= 'ok';
     { ----------- Do once for MoonRise (1), then set (2) ----------- }
     { --------------- Make sure lon is within +- 180 deg ----------- }
     IF Lon > Pi THEN
         Lon:= Lon - 2.0*Pi;
     IF Lon < -Pi THEN
         Lon:= Lon + 2.0*Pi;

     Try:= 1;
     Opt:= 1;
     WHILE Opt <= 2 DO
       BEGIN
         INVJDAY( JD,  Year,Month,Day,Hr,MIN,SEC );
         JDAY( Year,Month,Day,0,0,0.0, JDTemp );
         IF Show = 'Y' THEN
             WriteLn(FileOut,' JD at 0 hr ',JD:18:12 );
         UTTemp:= 0.5;  { days }

         IF Try = 2 THEN
             IF Opt = 1 THEN
                 UTTemp:= 0.25 { days }
               ELSE
                 UTTemp:= 0.75;  { days }

         tn:= UTTemp;  { Initial Guess }
         JDTemp:= JDTemp + UTTemp;
         i := 0;

         REPEAT
             TTDB := ( JDTemp - 2451545.0 ) / 36525.0;
             IF Show = 'Y' THEN
                 WriteLn(FileOut,'Option ',Opt:3,' JDTemp ',JDTemp:18:12,' TTDB ',TTDB:14:10 );
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
             EclpLong := MODFUNC( EclpLong*Deg2Rad, TwoPi );
             EclpLat  := MODFUNC( EclpLat*Deg2Rad, TwoPi );
             Obliquity:= 23.439291 - 0.0130042*TTDB; {deg}
             Obliquity:= Obliquity *Deg2Rad;
             IF Show = 'Y' THEN
                 WriteLn(FileOut,'Ecl lat,lon,obl ',EclpLat/deg2rad:11:7,eclpLong/deg2rad:11:7,obliquity/deg2rad:11:7 );

             { ------------- Find the geocentric direction cosines ---------- }
             l:= COS( EclpLat ) * COS( EclpLong );
             m:= COS(Obliquity)*COS(EclpLat)*SIN(EclpLong)
                 - SIN(Obliquity)*SIN(EclpLat);
             n:= SIN(Obliquity)*COS(EclpLat)*SIN(EclpLong)
                 + COS(Obliquity)*SIN(EclpLat);

             RtAsc:= ATAN2( m,l ); { more accurate than COS() TAN() }
             IF Show = 'Y' THEN
                 WriteLn(FileOut,'dir cosines lmn ',l:11:7,m:11:7,n:11:7,' rtasc ',rtasc/deg2rad:11:7 );
             { ---- Check that RtAsc is in the same quadrant as EclpLong ---- }
             IF EclpLong < 0.0 THEN
                 EclpLong:= EclpLong + TwoPi;   { make sure it's in 0 to 2Pi range }
             IF ABS( EclpLong - RtAsc ) > Pi*0.5 THEN
                 RtAsc:= RtAsc + 0.5*Pi*ROUND( (EclpLong-RtAsc)/(0.5*Pi) );

             Decl := ASIN( n );
             IF Show = 'Y' THEN
                 WriteLn(FileOut,'rtasc decl ',rtasc/deg2rad:11:7,decl/deg2rad:11:7 );

             LSTIME( Lon,JDTemp,LST,GST );
             MoonGHAn:= LST - Lon - RtAsc;
             IF Show = 'Y' THEN
                 WriteLn(FileOut,'LST GHAMn ',lst/deg2rad:11:7,Moonghan/deg2rad:11:7 );
             IF i = 0 THEN
               BEGIN
                 LHA := MoonGHAn + Lon;
                 DGHA:= 347.8 * Deg2Rad;
               END
               ELSE
                 DGHA:= (MoonGHAn - MoonGHA) / DeltaUT;
             IF Show = 'Y' THEN
                 WriteLn(FileOut,i:3,' LHA DGHA ',lha/deg2rad:11:7,dgha/deg2rad:11:7 );

             IF DGHA < 0.0 THEN
                 DGHA:= DGHA + TwoPi/ABS(DeltaUT);
             IF Show = 'Y' THEN
                 WriteLn(FileOut,' DGHA  ',DGHa/deg2rad:11:7,'same as before' );
             LHAn:= 0.00233 - (SIN(Latgd)*SIN(Decl))/(COS(Latgd)*COS(Decl));
             IF LHAn > 1.0 THEN
                 LHAn:= 0.0;
             IF LHAn < -1.0 THEN
                 LHAn:= -1.0;
             LHAn:= ACOS( LHAn );
             IF Show = 'Y' THEN
                 WriteLn(FileOut,' LHAn  ',lhan/deg2rad:11:7,' opt ',opt:3 );
             IF Opt = 1 THEN { MoonRise only }
                 LHAn:= TwoPi - LHAn;

             IF ABS( DGHA ) > 0.0001 THEN
                 DeltaUT:= (LHAn - LHA ) / DGHA
               ELSE
               BEGIN
                 DeltaUT:= (LHAn - LHA );  { ????}
                 DeltaUT:= 1.0;  { day }
                 Writeln( Fileout,'x');
               END;
             IF Show = 'Y' THEN
                 WriteLn(FileOut,' deltaut  ',DeltaUT:11:7 );
             t:= tn;
             IF ABS( DeltaUT ) > 0.5 THEN { diff day }
               BEGIN
                 IF ABS( DGHA ) > 0.001 THEN
                     IF DeltaUT < 0.0 THEN
                       BEGIN { day before }
                         DeltaUT:= DeltaUT + TwoPi/DGHA;
                         IF ABS( DeltaUT ) > 0.51 THEN
                             i:= 6;
                       END
                       ELSE
                       BEGIN { day after }
                         DeltaUT:= DeltaUT - TwoPi/DGHA;
                         IF ABS( DeltaUT ) > 0.51 THEN
                             i:= 6;
                       END
                   ELSE
                   BEGIN
                     DeltaUT:= DeltaUT;  { Take another try }
                     Write( Fileout,'y' );
                   END;
               END;
             IF Show = 'Y' THEN
                 WriteLn(FileOut,' deltaut  ',DeltaUT:11:7 );
             tn     := UTTemp + DeltaUT;
             JDTemp := JDTemp - UTTemp + tn;
             INC(i);
             MoonGHA:= MoonGHAn;
             IF Show = 'Y' THEN
                 WriteLn(FileOut,'end',i:3,' tn t ',tn:11:7,t:11:7,JDTemp:18:8 );
         UNTIL (ABS(tn-t) < 0.008) or (i > 5);

         UTTemp:= tn*24.0;
         IF i > 5 THEN
             UTTemp:= 9999.99;
         IF UTTemp < 9999.0 THEN
             UTTemp:= MODFUNC( UTTemp,24.0);
         IF UTTemp < 0.0 THEN
             UTTemp:= UTTEmp + 24.0;
         IF UTTemp > 900 THEN
             UTTemp:= 24.0;

         IF Show = 'Y' THEN
             WriteLn(FileOut,'opt',opt:3,' uttemp ',uttemp:11:7,' try ',try:3,' ------- ' );

         CASE Opt OF
           1 : UTMoonRise:= UTTemp;
           2 : UTMoonSet := UTTemp;
         END;  { Case }

         Try:= Try + 1;
         IF (i > 5) and (Try < 3) THEN { only make 2 guesses }
             Write { 'try #2 for ',opt:2 }
           ELSE
           BEGIN { go on to next calc }
             IF (i > 5) and (Try > 2) THEN
               BEGIN
                 IF Opt = 1 THEN
                     Error:= 'No Rise';
                 IF Opt = 2 THEN
                     Error:= 'No Set';
               END;
             Opt:= Opt + 1;
             Try:= 1;
           END;

       END;  { While Opt }

     { ------------- determine phase ANGLE of the MOON -------------- }
     MeanLong:= 280.4606184 + 36000.77005361*TTDB;
     MeanLong:= MODFUNC( MeanLong,360.0 ); {deg}

     MeanAnomaly:= 357.5277233 + 35999.05034*TTDB;
     MeanAnomaly:= MODFUNC( MeanAnomaly*Deg2Rad,TwoPi ); {rad}
     IF MeanAnomaly < 0.0 THEN
         MeanAnomaly:= TwoPi + MeanAnomaly;

     LonEclSun:= MeanLong + 1.914666471*SIN(MeanAnomaly)
                + 0.019994643*SIN(2.0*MeanAnomaly);{deg}

     LonEclMoon:=   218.32 + 481267.883*TTDB
                  + 6.29*Sin( (134.9+477198.85*TTDB)*Deg2Rad )
                  - 1.27*Sin( (259.2-413335.38*TTDB)*Deg2Rad )
                  + 0.66*Sin( (235.7+890534.23*TTDB)*Deg2Rad )
                  + 0.21*Sin( (269.9+954397.70*TTDB)*Deg2Rad )
                  - 0.19*Sin( (357.5+ 35999.05*TTDB)*Deg2Rad )
                  - 0.11*Sin( (186.6+966404.05*TTDB)*Deg2Rad );   { Deg }
     LonEclMoon:= MODFUNC( LonEclMoon, 360.0 );

     MoonPhaseAng:= LonEclMoon - LonEclSun;

     IF MoonPhaseAng < 0.0 THEN
         MoonPhaseAng:= 360.0 + MoonPhaseAng;   { deg }

   END;  { PROCEDURE MOONRISESET }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE HMS_SEC
|
|  this procedure converts Hours, minutes and Seconds into seconds from the
|    beginning of the day.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Hr          - Hours                          0 .. 24
|    MIN         - minutes                        0 .. 59
|    SEC         - Seconds                        0.0 .. 59.99
|    Direction   - Which set of vars to output    FROM  TOO
|
|  OutPuts       :
|    SEC         - Seconds                        0.0 .. 86400.0
|
|  Locals        :
|    Temp        - Temporary variable
|
|  Coupling      :
|    None.
|
 ----------------------------------------------------------------------------- }

PROCEDURE HMS_SEC           ( VAR Hr,MIN                             : INTEGER;
                              VAR SEC                                : EXTENDED;
                              Direction                              : Direct;
                              VAR UTSec                              : EXTENDED );
   VAR
     Temp : EXTENDED;
   BEGIN
     IF Direction = FROM THEN
       BEGIN
         Temp:= UTSec / 3600.0; { hr }
         Hr  := TRUNC( Temp );
         MIN := TRUNC( (Temp - Hr)*60.0 );
         SEC := (Temp - Hr - MIN/60.0 ) * 3600.0;
       END
       ELSE
         UTSec:= Hr*3600.0 + MIN*60.0 + SEC;
   END;  { PROCEDURE HMS_SEC }

{ ------------------------------------------------------------------------------
|
|                           PROCEDURE HMS_UT
|
|  this procedure converts Hours, minutes and Seconds into Universal Time.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Hr          - Hours                          0 .. 24
|    MIN         - minutes                        0 .. 59
|    SEC         - Seconds                        0.0 .. 59.99
|    Direction   - Which set of vars to output    FROM  TOO
|
|  OutPuts       :
|    UT          - Universal Time                 HrMin.SEC
|
|  Locals        :
|    None.
|
|  Coupling      :
|    None.
|
|  References    :
|    Vallado       2007, 205, alg 21, ex 3-10
|
 ----------------------------------------------------------------------------- }

PROCEDURE HMS_UT             ( VAR Hr,MIN                            : INTEGER;
                               VAR SEC                               : EXTENDED;
                               Direction                             : Direct;
                               VAR UT                                : EXTENDED );
   BEGIN
     IF Direction = FROM THEN
       BEGIN
         Hr := TRUNC( UT*0.01 );
         MIN:= TRUNC( UT - Hr*100.0 );
         SEC:= FRAC( UT ) * 100.0;
       END
       ELSE
         UT:= Hr*100.0 + MIN + SEC*0.01;
   END;  { PROCEDURE HMS_UT }
{}
{ ------------------------------------------------------------------------------
|
|                           PROCEDURE HMS_RAD
|
|  this procedure converts Hours, minutes and seconds into radians.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Hr          - Hours                          0 .. 24
|    MIN         - minutes                        0 .. 59
|    SEC         - Seconds                        0.0 .. 59.99
|    Direction   - Which set of vars to output    FROM  TOO
|
|  OutPuts       :
|    HMS         - Result                         rad
|
|  Locals        :
|    Temp        - Conversion from hours to rad   0.261799
|
|  Coupling      :
|    None.
|
|  References    :
|    Vallado       2007, 204, alg 19 alg 20, ex 3-9
|
 ----------------------------------------------------------------------------- }

PROCEDURE HMS_RAD            ( VAR Hr,MIN                            : INTEGER;
                               VAR SEC                               : EXTENDED;
                               Direction                             : Direct;
                               VAR HMS                               : EXTENDED );
   VAR
     Temp : EXTENDED;
   BEGIN
     Temp:= 15.0*Pi/180.0;
     IF Direction = FROM THEN
       BEGIN
         Temp:= HMS   / Temp;
         Hr  := TRUNC( Temp   );
         MIN := TRUNC( (Temp - Hr)*60.0 );
         SEC := (Temp - Hr - MIN/60.0 ) * 3600.0;
       END
       ELSE
         HMS:= ( Hr + MIN/60.0 + SEC/3600.0 )*Temp;
   END;  { PROCEDURE HMS_RAD }

{ ------------------------------------------------------------------------------
|
|                           PROCEDURE DMS_RAD
|
|  this procedure converts Degrees, minutes and seconds into radians.
|
|  Author        : David Vallado                  719-573-2600    1 Mar 2001
|
|  Inputs          Description                    Range / Units
|    Deg         - Degrees                        0 .. 360
|    MIN         - minutes                        0 .. 59
|    SEC         - Seconds                        0.0 .. 59.99
|    Direction   - Which set of vars to output    FROM  TOO
|
|  OutPuts       :
|    DMS         - Result                         rad
|
|  Locals        :
|    Temp        - Temporary variable
|
|  Coupling      :
|    None.
|
|  References    :
|    Vallado       2007, 203, alg 17 alg 18, ex 3-8
|
 ----------------------------------------------------------------------------- }

PROCEDURE DMS_RAD            ( VAR Deg,MIN                           : INTEGER;
                               VAR SEC                               : EXTENDED;
                               Direction                             : Direct;
                               VAR DMS                               : EXTENDED );
   CONST
     Rad2Deg    : EXTENDED =    180.0 / pi; { 57.29577951308230; }
     Deg2Rad    : EXTENDED =     pi / 180.0; { 0.01745329251994; }
   VAR
     Temp : EXTENDED;
   BEGIN
     IF Direction = FROM THEN
       BEGIN
         Temp:= DMS * Rad2Deg;
         Deg := TRUNC( Temp );
         MIN := TRUNC( (Temp - Deg)*60.0 );
         SEC := (Temp - Deg - MIN/60.0 ) * 3600.0;
       END
       ELSE
         DMS:= ( Deg + MIN/60.0 + SEC/3600.0 ) * Deg2Rad;
   END;  { PROCEDURE DMS_RAD }


BEGIN
  Show:= 'N';
  InitTime;
END.  { Unit AstTime }
{}
