        % -----------------------------------------------------------------------------
        %
        %                           function initjplde
        %
        %  this function initializes the jpl planetary ephemeris data. the input
        %  data files are from processing the ascii files into a text file of sun
        %  and moon positions.
        %
        %  author        : david vallado                  719-573-2600   22 jan 2018
        %
        %  revisions
        %
        %  inputs          description                    range / units
        %
        %
        %
        %
        %  outputs       :
        %    jpldearr    - array of jplde data records
        %    jdjpldestart- julian date of the start of the jpldearr data
        %
        %  locals        :
        %                -
        %
        %  coupling      :
        %
        %  references    :
        %  [jpldearr, jdjpldestart, jdjpldestartFrac] = initjplde('D:\Codes\LIBRARY\DataLib\sunmooneph_430t.txt');
        %  -------------------------------------------------------------------------- */

        function [jpldearr, jdjpldestart, jdjpldestartFrac] = initjplde(infilename)
        
            %double jdtdb, jdtdbf;
            %string pattern;
            %Int32 i;
            %jdjpldestart = 0.0;
            %jdjpldestartFrac = 0.0;

            % read the whole file at once into lines of an array
            filename = infilename;
            [fid,message] = fopen(filename,'r');
            fclose(fid);
            %fprintf(1,'%s \n',filename);
            filedat =load(filename);
            filedim = size(filedat,1); % just get # of rows
            %load data into x y z arrays
            jpldearr.year = filedat(:,1);
            jpldearr.mon = filedat(:,2);
            jpldearr.day = filedat(:,3);
            jpldearr.rsun1 = filedat(:,4);
            jpldearr.rsun2 = filedat(:,5);
            jpldearr.rsun3 = filedat(:,6);
            jpldearr.rsmag = filedat(:,7);
            jpldearr.rmoon1 = filedat(:,9);
            jpldearr.rmoon2 = filedat(:,10);
            jpldearr.rmoon3 = filedat(:,11);

            [jdtdb, jdtdbf] = jday(jpldearr.year, jpldearr.mon, jpldearr.day, 0, 0, 0.0);
            jpldearr.mjd = jdtdb + jdtdbf - 2400000.5;

            % ---- find epoch date
            [jdjpldestart, jdjpldestartFrac] = jday(jpldearr.year(1), jpldearr.mon(1), jpldearr.day(1), 0, 0, 0.0);
            jpldearr.mjd(1) = jdjpldestart + jdjpldestartFrac;

            %  initjplde
