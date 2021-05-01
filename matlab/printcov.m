        % ----------------------------------------------------------------------------
        %
        %                           function printcov
        %
        %  this function prints a covariance matrix
        %
        %  author        : david vallado                  719-573-2600   23 may 2003
        %
        %  revisions
        %
        %  inputs          description                    range / units
        %    covin       - 6x6 input covariance matrix
        %    covtype     - type of covariance             'cl','ct','fl','sp','eq',
        %    cu          - covariance units (deg or rad)  't' or 'm'
        %    anom        - anomaly                        'mean' or 'true' or 'tau'
        %
        %  outputs       :
        %
        %  locals        :
        %
        %  references    :
        %    none
        %
        %  printcov( covin, covtype, cu, anom )
        % ----------------------------------------------------------------------------

        function printcov( covin, covtype, cu, anom )

        if strcmp(anom,'truea') == 1 || strcmp(anom,'meana') == 1  % 1 is true
            semi = 'a m  ';
        else
            if strcmp(anom,'truen') == 1 || strcmp(anom,'meann') == 1  % 1 is true
                semi = 'n rad';
            end
        end

        if strcmp(covtype,'ct') == 1  % 1 is true
            fprintf(1,'cartesian covariance \n');
            fprintf(1,'        x  m            y m             z  m           xdot  m/s       ydot  m/s       zdot  m/s  \n');
        end;

        if strcmp(covtype,'cl') == 1  % 1 is true
            fprintf(1,'classical covariance \n');
            if (cu == 'm')
                fprintf(1,'          %s          ecc           incl rad      raan rad         argp rad        ', semi);
                if strcmp(anom,'meana') == 1 || strcmp(anom,'meann') == 1  % 1 is true
                    fprintf(1,' m rad \n');
                else
                    if strcmp(anom,'truea') == 1 || strcmp(anom,'truen') == 1  % 1 is true
                        fprintf(1,' nu rad \n');
                    end;
                end;
            else
                fprintf(1,'          %s           ecc           incl deg      raan deg         argp deg        ', semi);
                if strcmp(anom,'meana') == 1 || strcmp(anom,'meann') == 1  % 1 is true
                    fprintf(1,' m deg \n');
                else
                    if strcmp(anom,'truea') == 1 || strcmp(anom,'truen') == 1  % 1 is true
                        fprintf(1,' nu deg \n');
                    end;
                end;
            end;
        end;

        if strcmp(covtype,'eq') == 1  % 1 is true
            fprintf(1,'equinoctial covariance \n');
            %            if (cu == 'm')
            if strcmp(anom,'meana') == 1 || strcmp(anom,'meann') == 1  % 1 is true
                fprintf(1,'         %5s           af              ag           chi             psi         meanlonM rad\n',semi);
            else
                if strcmp(anom,'truea') == 1 || strcmp(anom,'truen') == 1  % 1 is true
                    fprintf(1,'         %5s           af              ag           chi             psi         meanlonNu rad\n',semi);
                end
            end;
        end;

        if strcmp(covtype,'fl') == 1  % 1 is true
            fprintf(1,'flight covariance \n');
            if (cu == 'm')
                fprintf(1,'       lon  rad      latgc rad        fpa rad         az rad           r  m           v  m/s  \n');
            else
                fprintf(1,'       lon  deg      latgc deg        fpa deg         az deg           r  m           v  m/s  \n');
            end;
        end;

        if strcmp(covtype,'sp') == 1  % 1 is true
            fprintf(1,'spherical covariance \n');
            if (cu == 'm')
                fprintf(1,'      rtasc rad       decl rad        fpa rad         az rad           r  m           v  m/s  \n');
            else
                fprintf(1,'      rtasc deg       decl deg        fpa deg         az deg           r  m           v  m/s  \n');
            end;
        end;

        %        fprintf(1,'%16e%16e%16e%16e%16e%16e\n',covin' );
        fprintf(1,'%20.10e%20.10e%20.10e%20.10e%20.10e%20.10e\n',covin' );



