Imports MyManager
Imports Annunci



Public Class ImmobiliareManager
    Inherits MyManager.Manager

    Private _mercatinoManager As MyManager.MercatinoManager



    'Public Enum Categoria
    '    Affitto = 1000000
    '    Scambio = 4000000
    '    Acquisto = 2000000
    '    Vendita = 3000000
    'End Enum

    'Public Enum TipoImmobile
    '    Appartamento = 1
    '    Monolocale = 2
    '    Attico = 3
    '    Villa = 4
    '    Casale = 5
    '    Box = 6
    '    Ufficio = 7
    '    Negozio = 8
    '    'aggiunti per RevoAgent
    '    Capannone = 9
    '    Terreno = 10
    '    Loft = 11
    '    Mansarda = 12
    '    Altro = 13
    'End Enum

    'Public Enum TipoCucina
    '    Undefined = -1
    '    No = 0
    '    Angolo_cottura = 1
    '    Semi_abitabile = 2
    '    Abitabile = 3
    '    Con_tinello = 4
    '    Altro = 5
    'End Enum

    'Public Enum TipoSalone
    '    Undefined = -1
    '    No = 0
    '    Singolo = 1
    '    Doppio = 2
    '    Triplo = 3
    '    Quadruplo = 4
    '    Altro = 5
    'End Enum

    'Public Enum TipoBoxAuto
    '    Undefined = -1
    '    No = 0
    '    Singolo = 1
    '    Doppio = 2
    '    Triplo = 3
    '    Altro = 4
    'End Enum

    'Public Enum TipoOccupazione
    '    Undefined = -1
    '    Libero = 1
    '    In_locazione = 2
    '    Nuda_proprieta = 3
    'End Enum

    'Public Enum TipoStatoImmobile
    '    Undefined = -1
    '    Ristrutturato = 1
    '    Da_ristrutturare = 2
    '    In_costruzione = 3
    'End Enum

    'Public Enum TipoRiscaldamento
    '    Undefined = -1
    '    No = 0
    '    Autonomo = 1
    '    Centralizzato = 2
    '    Centralizzato_ripartito = 3
    'End Enum

    'Public Enum TipoPostoAuto
    '    Undefined = -1
    '    No = 0
    '    Singolo_scoperto = 1
    '    Singolo_coperto = 2
    '    Doppio_scoperto = 3
    '    Doppio_coperto = 4
    '    Triplo_scoperto = 5
    '    Triplo_coperto = 6
    '    Altro = 7
    'End Enum


    ''24/01/2012
    'Public Enum SourceId
    '    Undefined = -1
    '    RevoAgent = 1
    'End Enum



    Public _nota As String 'nota/descizione
    Public _categoria As Models.Immobile.Categorie

    Public _prezzo As Decimal
    Public _regione As String
    Public _provincia As String
    Public _comune As String

    Public _regioneId As Long
    Public _provinciaId As String
    Public _comuneId As String

    Public _tipoImmobile As Models.Immobile.TipoImmobile
    Public _mq As Integer
    Public _piano As Integer
    Public _pianiTotali As Integer
    Public _anno As Integer
    Public _cucina As Models.Immobile.TipoCucina
    Public _salone As Models.Immobile.TipoSalone
    '  Public _stato As String
    Public _indirizzo As String
    Public _quartiere As String
    Public _cap As String
    Public _camereDaLetto As Integer
    Public _camerette As Integer
    Public _bagni As Integer
    Public _terrazzi As Integer
    Public _balconi As Integer
    Public _giardinoMq As Integer
    Public _cantina As Integer
    Public _soffitta As Integer

    Public _postoAuto As Models.Immobile.TipoPostoAuto
    Public _box As Models.Immobile.TipoBoxAuto
    Public _occupazione As Models.Immobile.TipoOccupazione
    Public _statoImmobile As Models.Immobile.TipoStatoImmobile

    Public _speseCondominiali As Decimal
    Public _riscaldamento As Models.Immobile.TipoRiscaldamento
    Public _climatizzato As Integer
    Public _ascensore As Integer
    Public _portiere As Integer

    Public _latitude As Double
    Public _longitude As Double

    Public _classeEnergetica As String

    '24/01/2012 
    Public _externalId As String
    Public _sourceId As Models.Immobile.TipoSourceId



    Public Sub New()
        MyBase.New("immobiliare")
        _mercatinoManager = New MyManager.MercatinoManager(Me.mConnection)
    End Sub

    Public Sub New(ByVal connectionName As String)
        MyBase.New(connectionName)
        _mercatinoManager = New MyManager.MercatinoManager(Me.mConnection)
    End Sub

    Public Sub New(ByVal connection As System.Data.Common.DbConnection)
        MyBase.New(connection)
        _mercatinoManager = New MyManager.MercatinoManager(Me.mConnection)
    End Sub

    Public Function insertAnnuncio(ByVal userId As Long) As Long
        Return insertAnnuncio(userId, False, 0, Date.MinValue)
    End Function

    Public Function insertAnnuncioInTestMode(ByVal userId As Long) As Long
        Return insertAnnuncio(userId, True, 0, Date.MinValue)
    End Function

    Public Function insertAnnuncio(ByVal userId As Long, test_mode As Boolean, myStato As AnnunciManager.StatoAnnuncio, dateAdded As DateTime) As Long

        If _categoria = 0 OrElse IsNothing(_categoria) Then
            Throw New MyManager.ManagerException("La Categoria deve essere obbiligatoria")
        End If

        Dim strSQL As String
        Dim strSQLParametri As String

        strSQL = "INSERT INTO ANNUNCIO ( FK_CATEGORIA_ID , MY_STATO"
        strSQLParametri = " VALUES ( " & _categoria
        If IsNothing(myStato) OrElse myStato = 0 Then
            strSQLParametri &= ", '" & AnnunciManager.StatoAnnuncio.Pubblicato.ToString() & "' "
        Else
            strSQLParametri &= ", '" & myStato.ToString() & "' "
        End If


        Dim command As System.Data.Common.DbCommand
        command = mConnection.CreateCommand()

        '27/01/2012 iposto data di modifica =  data inserimento 
        Dim dataCorrente As DateTime = DateTime.Now
        strSQL &= ",DATE_ADDED, DATE_MODIFIED"
        strSQLParametri &= ", @DATE_ADDED , @DATE_MODIFIED "

        If dateAdded <> Date.MinValue Then
            mAddParameter(command, "@DATE_ADDED", dateAdded)
            mAddParameter(command, "@DATE_MODIFIED", dateAdded)
        Else
            mAddParameter(command, "@DATE_ADDED", dataCorrente)
            mAddParameter(command, "@DATE_MODIFIED", dataCorrente)
        End If

        If userId <> 0 Then
            strSQL &= ",FK_USER_ID "
            strSQLParametri &= ", @FK_USER_ID "
            mAddParameter(command, "@FK_USER_ID", userId)
        End If

        strSQL &= ",TIPO "
        strSQLParametri &= ", @TIPO "
        mAddParameter(command, "@TIPO", _tipoImmobile.ToString())

        If _nota <> "" Then
            strSQL &= ",DESCRIZIONE "
            strSQLParametri &= ", @DESCRIZIONE "
            mAddParameter(command, "@DESCRIZIONE", _nota)
        End If

        If _mq > 0 Then
            strSQL &= ",MQ "
            strSQLParametri &= ", @MQ "
            mAddParameter(command, "@MQ", _mq)
        End If

        If _piano <> Integer.MinValue Then
            strSQL &= ",PIANO "
            strSQLParametri &= ", @PIANO "
            mAddParameter(command, "@PIANO", _piano)
        End If

        If _pianiTotali <> Integer.MinValue Then
            strSQL &= ",PIANI_TOTALI "
            strSQLParametri &= ", @PIANI_TOTALI "
            mAddParameter(command, "@PIANI_TOTALI", _pianiTotali)
        End If

        If _anno > 0 Then
            strSQL &= ",ANNO "
            strSQLParametri &= ", @ANNO "
            mAddParameter(command, "@ANNO", _anno)
        End If

        If _cucina <> Models.Immobile.TipoCucina.Undefined Then
            strSQL &= ",CUCINA "
            strSQLParametri &= ", @CUCINA "
            'mAddParameter(command, "@CUCINA", _cucina.ToString.Replace("_", " "))
            mAddParameter(command, "@CUCINA", _cucina.ToString())
        End If


        If _salone <> Models.Immobile.TipoSalone.Undefined Then
            strSQL &= ",SALONE "
            strSQLParametri &= ", @SALONE "
            mAddParameter(command, "@SALONE", _salone.ToString)
        End If


        If _statoImmobile <> Models.Immobile.TipoStatoImmobile.Undefined Then
            strSQL &= ",STATO "
            strSQLParametri &= ", @STATO "
            'mAddParameter(command, "@STATO", _statoImmobile.ToString.Replace("_", " "))
            mAddParameter(command, "@STATO", _statoImmobile.ToString())
        End If


        If _occupazione <> Models.Immobile.TipoOccupazione.Undefined Then
            strSQL &= ",OCCUPAZIONE "
            strSQLParametri &= ", @OCCUPAZIONE "
            'mAddParameter(command, "@OCCUPAZIONE", _occupazione.ToString.Replace("_", " "))
            mAddParameter(command, "@OCCUPAZIONE", _occupazione.ToString())
        End If


        If _box <> Models.Immobile.TipoBoxAuto.Undefined Then
            strSQL &= ",BOX "
            strSQLParametri &= ", @BOX "
            mAddParameter(command, "@BOX", _box.ToString)
        End If

        If _postoAuto <> Models.Immobile.TipoPostoAuto.Undefined Then
            strSQL &= ",POSTO_AUTO "
            strSQLParametri &= ", @POSTO_AUTO "
            'mAddParameter(command, "@POSTO_AUTO", _postoAuto.ToString.Replace("_", " "))
            mAddParameter(command, "@POSTO_AUTO", _postoAuto.ToString())
        End If


        If _prezzo > 0 Then
            strSQL &= ",PREZZO "
            strSQLParametri &= ", @PREZZO "
            mAddParameter(command, "@PREZZO", _prezzo)
        End If

        If Not String.IsNullOrEmpty(_indirizzo) Then
            strSQL &= ",INDIRIZZO "
            strSQLParametri &= ", @INDIRIZZO "
            mAddParameter(command, "@INDIRIZZO", _indirizzo)
        End If

        If Not String.IsNullOrEmpty(_regione) Then
            strSQL &= ",REGIONE "
            strSQLParametri &= ", @REGIONE "
            mAddParameter(command, "@REGIONE", _regione)
        End If

        If Not String.IsNullOrEmpty(_provincia) Then
            strSQL &= ",PROVINCIA "
            strSQLParametri &= ", @PROVINCIA "
            mAddParameter(command, "@PROVINCIA", _provincia)
        End If

        If Not String.IsNullOrEmpty(_comune) Then
            strSQL &= ", COMUNE "
            strSQLParametri &= ", @COMUNE "
            mAddParameter(command, "@COMUNE", _comune)
        End If


        If _regioneId <> -1 Then
            strSQL &= ",REGIONE_ID "
            strSQLParametri &= ", @REGIONE_ID "
            mAddParameter(command, "@REGIONE_ID", _regioneId)
        End If

        'If _provinciaId <> -1 Then
        If Not String.IsNullOrEmpty(_provinciaId) Then
            strSQL &= ",PROVINCIA_ID "
            strSQLParametri &= ", @PROVINCIA_ID "
            mAddParameter(command, "@PROVINCIA_ID", _provinciaId)
        End If

        'If _comuneId <> -1 Then
        If Not String.IsNullOrEmpty(_comuneId) Then
            strSQL &= ", COMUNE_ID "
            strSQLParametri &= ", @COMUNE_ID "
            mAddParameter(command, "@COMUNE_ID", _comuneId)
        End If



        If Not String.IsNullOrEmpty(_quartiere) Then
            strSQL &= ",QUARTIERE "
            strSQLParametri &= ", @QUARTIERE "
            mAddParameter(command, "@QUARTIERE", _quartiere)
        End If

        If Not String.IsNullOrEmpty(_cap) Then
            strSQL &= ",CAP "
            strSQLParametri &= ", @CAP "
            mAddParameter(command, "@CAP", _cap)
        End If

        If _camereDaLetto <> -1 Then
            strSQL &= ",CAMERE_DA_LETTO "
            strSQLParametri &= ", @CAMERE_DA_LETTO "
            mAddParameter(command, "@CAMERE_DA_LETTO", _camereDaLetto)
        End If

        If _camerette <> -1 Then
            strSQL &= ",CAMERETTE "
            strSQLParametri &= ", @CAMERETTE "
            mAddParameter(command, "@CAMERETTE", _camerette)
        End If

        If _bagni <> -1 Then
            strSQL &= ",BAGNI "
            strSQLParametri &= ", @BAGNI "
            mAddParameter(command, "@BAGNI", _bagni)
        End If

        If _balconi <> -1 Then
            strSQL &= ",BALCONI "
            strSQLParametri &= ", @BALCONI "
            mAddParameter(command, "@BALCONI", _balconi)
        End If

        If _terrazzi <> -1 Then
            strSQL &= ",TERRAZZI "
            strSQLParametri &= ", @TERRAZZI "
            mAddParameter(command, "@TERRAZZI", _terrazzi)
        End If


        If _giardinoMq <> Integer.MinValue Then
            strSQL &= ",GIARDINO_MQ "
            strSQLParametri &= ", @GIARDINO_MQ "
            mAddParameter(command, "@GIARDINO_MQ", _giardinoMq)
        End If


        If _cantina <> -1 Then
            strSQL &= ",CANTINA "
            strSQLParametri &= ", @CANTINA "
            mAddParameter(command, "@CANTINA", _cantina)
        End If

        If _soffitta <> -1 Then
            strSQL &= ",SOFFITTA "
            strSQLParametri &= ", @SOFFITTA "
            mAddParameter(command, "@SOFFITTA", _soffitta)
        End If


        If _climatizzato <> -1 Then
            strSQL &= ",CLIMATIZZATO "
            strSQLParametri &= ", @CLIMATIZZATO "
            mAddParameter(command, "@CLIMATIZZATO", _climatizzato)
        End If


        If _ascensore <> -1 Then
            strSQL &= ",ASCENSORE "
            strSQLParametri &= ", @ASCENSORE "
            mAddParameter(command, "@ASCENSORE", _ascensore)
        End If

        If _portiere <> -1 Then
            strSQL &= ",PORTIERE "
            strSQLParametri &= ", @PORTIERE "
            mAddParameter(command, "@PORTIERE", _portiere)
        End If

        If _speseCondominiali > 0 Then
            strSQL &= ",SPESE_CONDOMINIALI "
            strSQLParametri &= ", @SPESE_CONDOMINIALI "
            mAddParameter(command, "@SPESE_CONDOMINIALI", _speseCondominiali)
        End If

        'REL. 1.0.0.7 del 05/01/2012
        If Not String.IsNullOrEmpty(_classeEnergetica) Then
            strSQL &= ",CLASSE_ENERGETICA "
            strSQLParametri &= ", @CLASSE_ENERGETICA "
            mAddParameter(command, "@CLASSE_ENERGETICA", _classeEnergetica)
        End If



        If _riscaldamento <> Models.Immobile.TipoRiscaldamento.Undefined Then
            strSQL &= ",RISCALDAMENTO "
            strSQLParametri &= ", @RISCALDAMENTO "
            'mAddParameter(command, "@RISCALDAMENTO", _riscaldamento.ToString.Replace("_", " "))
            mAddParameter(command, "@RISCALDAMENTO", _riscaldamento.ToString())
        End If


        If _latitude <> 0 Then
            strSQL &= ",LATITUDE "
            strSQLParametri &= "," & _latitude.ToString("G18").Replace(",", ".")


            '25/02/2011  utilizzando i parametri mi tronca i valori!!

            'strSQLParametri &= ", @LATITUDE "
            'mAddParameter(command, "@LATITUDE", _latitude)
        End If

        If _longitude <> 0 Then
            strSQL &= ",LONGITUDE "

            strSQLParametri &= "," & _longitude.ToString("G18").Replace(",", ".")
            'strSQLParametri &= ", @LONGITUDE "
            'mAddParameter(command, "@LONGITUDE", _longitude)
        End If



        '24/01/2012
        If _sourceId <> Models.Immobile.TipoSourceId.Undefined Then
            strSQL &= ",SOURCE_ID "
            strSQLParametri &= ", @SOURCE_ID "
            mAddParameter(command, "@SOURCE_ID", Integer.Parse(_sourceId))
        End If

        If Not String.IsNullOrEmpty(_externalId) Then
            strSQL &= ",EXTERNAL_ID "
            strSQLParametri &= ", @EXTERNAL_ID "
            mAddParameter(command, "@EXTERNAL_ID", _externalId)
        End If


        command.CommandText = strSQL & " ) " & strSQLParametri & " )"

        If test_mode = True Then
            Me._transactionBegin()
            mExecuteNoQuery(command)
            Me._transactionRollback()
            Return -1
        End If

        mExecuteNoQuery(command)

        Return _getIdentity()
    End Function

    Public Function countPhoto(ByVal annuncioId As Long) As Integer
        Return _mercatinoManager.countPhoto(annuncioId)
    End Function


    Public Function getPhoto(ByVal annuncioId As Long) As Data.DataTable
        mStrSQL = "SELECT * FROM PHOTO WHERE  FK_EXTERNAL_ID= " & annuncioId
        Return Me.mFillDataTable(mStrSQL)
    End Function


    Public Function getPhotoPlanimetria(ByVal annuncioId As Long) As Data.DataTable
        mStrSQL = "SELECT * FROM PHOTO WHERE  DESCRIPTION = 'PLANIMETRIA' AND FK_EXTERNAL_ID= " & annuncioId
        Return Me.mFillDataTable(mStrSQL)
    End Function

    Public Function deleteAllPhoto(annuncioId As Long) As Long
        mStrSQL = "DELETE * FROM PHOTO WHERE  FK_EXTERNAL_ID= " & annuncioId
        Return mExecuteNoQuery(mStrSQL)
    End Function


    Public Function insertPhoto(ByVal absolutePath As String, ByVal description As String, ByVal externalId As Long, ByVal userId As Long) As Long
        Dim photo As New MyManager.PhotoManager(mConnection)
        Return photo.insertPhoto(absolutePath, description, externalId, userId)
    End Function


    Public Function getAnnuncioByExternalId(ByVal externalId As String) As Data.DataTable
        Dim m As Annunci.AnnunciManager = New AnnunciManager(mConnection)
        Return m.getAnnuncioByExternalId(externalId)
    End Function


    Public Function getAnnuncio(ByVal annuncioId As Long) As Data.DataSet
        mStrSQL = "SELECT UTENTI.my_login AS my_login, UTENTI.user_id AS user_id, UTENTI.isModeratore AS isModeratore " &
            ", ANNUNCIO.*  " &
            ", categorie.nome AS categoria " &
            " FROM categorie INNER JOIN (ANNUNCIO LEFT JOIN UTENTI ON ANNUNCIO.fk_user_id=UTENTI.user_id) ON categorie.categoria_id=ANNUNCIO.fk_categoria_id " &
            " WHERE ( ANNUNCIO.date_deleted Is Null) And ANNUNCIO.ANNUNCIO_ID = " & annuncioId

        Return Me._fillDataSet(mStrSQL)
    End Function


    Public Sub resetContatoreParziale(ByVal annuncioId As Long)
        _mercatinoManager.resetContatoreParziale(annuncioId)
    End Sub

    Public Sub annuncioAddClick(ByVal annuncioId As Long)
        _mercatinoManager.annuncioAddClick(annuncioId)
    End Sub

    Public Function getTrattativeOnMyAnnuncio(ByVal userId As Long, ByVal annuncioId As Long) As Data.DataTable
        Return _mercatinoManager.getTrattativeOnMyAnnuncio(userId, annuncioId)
    End Function

    Public Function getNumeroRisposteOfTrattativa(ByVal trattativaId As Long, ByVal userId As Long) As Long
        Return _mercatinoManager.getNumeroRisposteOfTrattativa(trattativaId, userId)
    End Function


    Public Function getEmailUtentiInTrattativa(ByVal annuncio_id As Long) As Data.DataTable
        Dim m As Annunci.AnnunciManager = New AnnunciManager(mConnection)
        Return m.getEmailUtentiInTrattativa(annuncio_id)

        'Return _mercatinoManager.getEmailUtentiInTrattativa(annuncio_id)
    End Function

    Public Function getStatoTrattativa(ByVal trattativaId As Long) As MyManager.MercatinoManager.StatoTrattativa
        Return _mercatinoManager.getStatoTrattativa(trattativaId)
    End Function

    Public Function deleteTrattativaLogic(ByVal trattativaId As Long, ByVal userId As Long) As Boolean
        Dim m As Annunci.AnnunciManager = New AnnunciManager(mConnection)
        Return m.deleteTrattativaLogic(trattativaId, userId)

        'Return _mercatinoManager.deleteTrattativaLogic(trattativaId, userId)
    End Function

    Public Function insertUser(ByVal userId As Long, ByVal nome As String, ByVal cognome As String, ByVal email As String, ByVal mylogin As String, ByVal customerId As Long) As Long
        Return _mercatinoManager.insertUser(userId, nome, cognome, email, mylogin, customerId)
    End Function

    Public Function getTitoloOfAnnuncio(ByVal annuncioId As Long) As String
        mStrSQL = "SELECT categorie.nome & ' - ' &   ANNUNCIO.tipo  AS titolo" &
            " FROM categorie INNER JOIN ANNUNCIO  ON categorie.categoria_id=ANNUNCIO.fk_categoria_id" &
            " WHERE ANNUNCIO_ID=" & annuncioId
        Return mExecuteScalar(mStrSQL)
    End Function

    Public Function updateLatitude(ByVal value As Double) As Long

        'mStrSql = "UPDATE ANNUNCIO SET LATITUDE =  " & value.ToString("G19").Replace(",", ".") & " WHERE annuncio_id = " & 1295542737
        ' Return mExecuteNoQuery(mStrSql)




        '   mStrSql = "UPDATE ANNUNCIO SET LATITUDE = @LATITUDE WHERE annuncio_id = " & 1295542737

        Dim command As System.Data.Common.DbCommand
        command = mConnection.CreateCommand()
        command.CommandText = mStrSQL

        mAddParameter(command, "@LATITUDE", value)
        Return mExecuteNoQuery(command)
    End Function


    Public Function updateEmail(ByVal userId As Long, ByVal email As String) As Boolean
        mStrSQL = "UPDATE UTENTI SET DATE_MODIFIED = NOW ,EMAIL = @email " &
                                                     " WHERE USER_ID=" & userId

        Dim command As System.Data.Common.DbCommand
        command = mConnection.CreateCommand()
        command.CommandText = mStrSQL

        Me.mAddParameter(command, "@email", email)
        Me.mExecuteNoQuery(command)

        Return True
    End Function




    '*** nuove funzioni per RevoAgent ***

    Public Function countAnnunciWithSourceExternal(ByVal userId As Long) As Integer
        mStrSQL = "SELECT count(*) FROM ANNUNCIO WHERE SOURCE_ID is not null AND FK_USER_ID= " & userId
        Return Me.mExecuteScalar(mStrSQL)
    End Function

    Public Function countAnnunci(ByVal userId As Long, stato As AnnunciManager.StatoAnnuncio) As Integer
        mStrSQL = "SELECT count(*) FROM ANNUNCIO WHERE SOURCE_ID is null AND FK_USER_ID= " & userId &
            " AND MY_STATO ='" & stato.ToString & "' and DATE_DELETED IS NULL"
        Return Me.mExecuteScalar(mStrSQL)
    End Function

    Public Function countAnnunciWithDateDateted(ByVal userId As Long) As Integer
        mStrSQL = "SELECT count(*) FROM ANNUNCIO WHERE SOURCE_ID is null AND FK_USER_ID= " & userId &
            " AND DATE_DELETED is not null"
        Return Me.mExecuteScalar(mStrSQL)
    End Function

    Public Function getUtente(ByVal userId As Long) As Data.DataTable
        Return _mercatinoManager.getUtente(userId)
    End Function

    Public Function updateUtente(ByVal userId As Long, ByVal externalId As String, sourceId As Models.Immobile.TipoSourceId, stato As Models.Immobile.StatoUtente, nota As String) As Boolean

        Dim command As System.Data.Common.DbCommand
        command = mConnection.CreateCommand()

        If sourceId = Models.Immobile.TipoSourceId.Undefined Then
            mStrSQL = "UPDATE UTENTI SET DATE_MODIFIED = NOW ,EXTERNAL_ID = NULL , SOURCE_ID = NULL "
        Else
            mStrSQL = "UPDATE UTENTI SET DATE_MODIFIED = NOW ,EXTERNAL_ID = @EXTERNAL_ID , SOURCE_ID =" & sourceId
            Me.mAddParameter(command, "@EXTERNAL_ID", externalId)
        End If

        If stato = Models.Immobile.StatoUtente.Undefined Then
            mStrSQL &= ",STATO_ID = NULL , DATE_STATO = NULL "
        Else
            mStrSQL &= ",STATO_ID = " & stato & " , DATE_STATO = NOW"
        End If

        If String.IsNullOrEmpty(nota) Then
            mStrSQL &= ",NOTA = NULL "
        Else
            mStrSQL &= ",NOTA = @NOTA "
            Me.mAddParameter(command, "@NOTA", nota)
        End If

        mStrSQL &= " WHERE USER_ID = " & userId
        command.CommandText = mStrSQL

        Me.mExecuteNoQuery(command)
        Return True
    End Function



    Public Function updateStatoAnnunciBySourceIdAndStatoIniziale(sourceId As Models.Immobile.TipoSourceId, statoSource As AnnunciManager.StatoAnnuncio, statoNew As AnnunciManager.StatoAnnuncio) As Integer
        mStrSQL = "UPDATE ANNUNCIO SET MY_STATO = '" & statoNew.ToString & "'"
        mStrSQL &= " WHERE EXTERNAL_ID is not null AND SOURCE_ID = " & sourceId & " AND MY_STATO = '" & statoSource.ToString & "' "
        Return mExecuteNoQuery(mStrSQL)
    End Function


    Public Function updateStatoAnnunciBySourceId(sourceId As Models.Immobile.TipoSourceId, stato As Annunci.AnnunciManager.StatoAnnuncio) As Integer
        Dim m As Annunci.AnnunciManager = New AnnunciManager(mConnection)
        Return m.updateStatoAnnunciBySourceId(sourceId, stato)


        'mStrSQL = "UPDATE ANNUNCIO SET MY_STATO = '" & stato.ToString & "'"
        'mStrSQL &= " WHERE EXTERNAL_ID is not null AND SOURCE_ID = " & sourceId
        'Return mExecuteNoQuery(mStrSQL)
    End Function

    Public Function updateStatoAnnuncio(ByVal annuncioId As Long, stato As AnnunciManager.StatoAnnuncio) As Integer
        mStrSQL = "UPDATE ANNUNCIO SET MY_STATO = '" & stato.ToString & "'"
        mStrSQL &= " WHERE ANNUNCIO_ID=" & annuncioId
        Return mExecuteNoQuery(mStrSQL)
    End Function

    Public Function getUtenteByExternalId(ByVal externalId As String, sourceId As Models.Immobile.TipoSourceId) As Data.DataTable
        mStrSQL = "SELECT *" &
                    " FROM UTENTI " &
                    " WHERE EXTERNAL_ID = '" & externalId & "' AND SOURCE_ID = " & sourceId

        Return Me.mFillDataTable(mStrSQL)
    End Function

    Public Function updatePianoAnnuncio(ByVal annuncioId As Long, piano As Integer) As Integer
        mStrSQL = "UPDATE ANNUNCIO SET PIANO = " & piano
        mStrSQL &= " WHERE ANNUNCIO_ID=" & annuncioId
        Return mExecuteNoQuery(mStrSQL)
    End Function

    Public Function updateRegioneProvinciaComuneAnnuncio(ByVal annuncioId As Long) As Integer
        mStrSQL = "UPDATE ANNUNCIO SET REGIONE = @REGIONE "
        mStrSQL &= " ,REGIONE_ID = @REGIONE_ID "
        mStrSQL &= " ,PROVINCIA = @PROVINCIA "
        mStrSQL &= " ,PROVINCIA_ID = @PROVINCIA_ID "
        mStrSQL &= " ,COMUNE = @COMUNE "
        mStrSQL &= " ,COMUNE = @COMUNE "


        mStrSQL &= " WHERE ANNUNCIO_ID=" & annuncioId
        Return mExecuteNoQuery(mStrSQL)
    End Function


    Public Function updateAnnuncioDescrizione(ByVal annuncioId As Long, ByVal descrizione As String, test_mode As Boolean) As Integer
        Return _mercatinoManager.updateAnnuncioDescrizione(annuncioId, descrizione, test_mode)
    End Function

    Public Function updateAnnuncioPrezzo(ByVal annuncioId As Long, prezzo As Decimal, test_mode As Boolean) As Integer
        Return _mercatinoManager.updateAnnuncioPrezzo(annuncioId, prezzo, test_mode)
    End Function

    Public Function updateAnnuncio(ByVal annuncioId As Long, test_mode As Boolean) As Integer
        mStrSQL = "UPDATE ANNUNCIO SET DATE_MODIFIED = NOW  "

        Dim command As System.Data.Common.DbCommand
        command = mConnection.CreateCommand()

        'mStrSql &= " ,DESCRIZIONE = @DESCRIZIONE "
        'If Not String.IsNullOrEmpty(_nota) Then
        '    mAddParameter(command, "@DESCRIZIONE", _nota)
        'Else
        '    mAddParameter(command, "@DESCRIZIONE", DBNull.Value)
        'End If

        mStrSQL &= " ,MQ = @MQ "
        If _mq > 0 Then
            mAddParameter(command, "@MQ", _mq)
        Else
            mAddParameter(command, "@MQ", DBNull.Value)
        End If

        mStrSQL &= " ,PIANO = @PIANO "
        If _piano <> Integer.MinValue Then
            mAddParameter(command, "@PIANO", _piano)
        Else
            mAddParameter(command, "@PIANO", DBNull.Value)
        End If

        mStrSQL &= " ,PIANI_TOTALI = @PIANI_TOTALI "
        If _pianiTotali <> Integer.MinValue Then
            mAddParameter(command, "@PIANI_TOTALI", _pianiTotali)
        Else
            mAddParameter(command, "@PIANI_TOTALI", DBNull.Value)
        End If

        mStrSQL &= " ,ANNO = @ANNO "
        If _anno > 0 Then
            mAddParameter(command, "@ANNO", _anno)
        Else
            mAddParameter(command, "@ANNO", DBNull.Value)
        End If

        mStrSQL &= " ,CUCINA = @CUCINA "
        If _cucina <> Models.Immobile.TipoCucina.Undefined Then
            'mAddParameter(command, "@CUCINA", _cucina.ToString.Replace("_", " "))
            mAddParameter(command, "@CUCINA", _cucina.ToString())
        Else
            mAddParameter(command, "@CUCINA", DBNull.Value)
        End If

        mStrSQL &= " ,SALONE = @SALONE "
        If _salone <> Models.Immobile.TipoSalone.Undefined Then
            'mAddParameter(command, "@SALONE", _salone.ToString.Replace("_", " "))
            mAddParameter(command, "@SALONE", _salone.ToString())
        Else
            mAddParameter(command, "@SALONE", DBNull.Value)
        End If

        mStrSQL &= " ,STATO = @STATO "
        If _statoImmobile <> Models.Immobile.TipoStatoImmobile.Undefined Then
            'mAddParameter(command, "@STATO", _statoImmobile.ToString.Replace("_", " "))
            mAddParameter(command, "@STATO", _statoImmobile.ToString())
        Else
            mAddParameter(command, "@STATO", DBNull.Value)
        End If

        mStrSQL &= " ,OCCUPAZIONE = @OCCUPAZIONE "
        If _occupazione <> Models.Immobile.TipoOccupazione.Undefined Then
            'mAddParameter(command, "@OCCUPAZIONE", _occupazione.ToString.Replace("_", " "))
            mAddParameter(command, "@OCCUPAZIONE", _occupazione.ToString())
        Else
            mAddParameter(command, "@OCCUPAZIONE", DBNull.Value)
        End If

        mStrSQL &= " ,REGIONE = @REGIONE "
        mAddParameter(command, "@REGIONE", _regione)

        mStrSQL &= " ,REGIONE_ID = @REGIONE_ID "
        mAddParameter(command, "@REGIONE_ID", _regioneId)

        mStrSQL &= " ,PROVINCIA = @PROVINCIA "
        mAddParameter(command, "@PROVINCIA", _provincia)

        mStrSQL &= " ,PROVINCIA_ID = @PROVINCIA_ID "
        mAddParameter(command, "@PROVINCIA_ID", _provinciaId)

        mStrSQL &= " ,COMUNE = @COMUNE "
        mAddParameter(command, "@COMUNE", _comune)

        mStrSQL &= " ,COMUNE_ID = @COMUNE_ID "
        mAddParameter(command, "@COMUNE_ID", _comuneId)

        If _latitude <> 0 Then
            mStrSQL &= ",LATITUDE =" & _latitude.ToString("G18").Replace(",", ".")
        Else
            mStrSQL &= ",LATITUDE = NULL"
        End If

        If _longitude <> 0 Then
            mStrSQL &= ",LONGITUDE =" & _longitude.ToString("G18").Replace(",", ".")
        Else
            mStrSQL &= ",LONGITUDE = NULL"
        End If

        mStrSQL &= " ,CLASSE_ENERGETICA = @CLASSE_ENERGETICA "
        If String.IsNullOrEmpty(_classeEnergetica) Then
            mAddParameter(command, "@CLASSE_ENERGETICA", DBNull.Value)
        Else
            mAddParameter(command, "@CLASSE_ENERGETICA", _classeEnergetica)
        End If


        'mStrSql &= " ,PREZZO = @PREZZO "
        'If _prezzo <> Decimal.MinValue Then
        '    mAddParameter(command, "@PREZZO", _prezzo)
        'Else
        '    mAddParameter(command, "@PREZZO", DBNull.Value)
        'End If

        mStrSQL &= " WHERE ANNUNCIO_ID=" & annuncioId

        command.CommandText = mStrSQL

        If test_mode = True Then
            Me._transactionBegin()
            mExecuteNoQuery(command)
            Me._transactionRollback()
            Return -1
        End If

        Return mExecuteNoQuery(command)
    End Function

    Public Function getAnnunciExternalBySource(sourceId As Models.Immobile.TipoSourceId) As Data.DataTable
        mStrSQL = "SELECT ANNUNCIO.*  " &
                    " FROM ANNUNCIO " &
                    " WHERE ( EXTERNAL_ID IS NOT NULL) And SOURCE_ID = " & sourceId

        Return Me.mFillDataTable(mStrSQL)
    End Function

    Public Function getAnnunciExternalBySourceAndStato(sourceId As Models.Immobile.TipoSourceId, stato As AnnunciManager.StatoAnnuncio) As Data.DataTable
        mStrSQL = "SELECT ANNUNCIO.*  " &
                    " FROM ANNUNCIO " &
                    " WHERE ( EXTERNAL_ID IS NOT NULL) And SOURCE_ID = " & sourceId & " AND MY_STATO = '" & stato.ToString() & "' "

        Return Me.mFillDataTable(mStrSQL)
    End Function

    Public Function deleteAnnuncioLogic(ByVal annuncioId As Long, ByVal absoluteServerPath As String) As Boolean
        Dim m As Annunci.AnnunciManager = New AnnunciManager(mConnection)
        Return m.deleteAnnuncioLogic(annuncioId, AnnunciManager.StatoAnnuncio.Da_cancellare, absoluteServerPath)

        'Return _mercatinoManager.deleteAnnuncioLogic(annuncioId, MercatinoManager.StatoAnnuncio.DaCancellare, absoluteServerPath)
    End Function

    'Public Function deleteTrattativaLogicByAnnuncio(ByVal trattativaId As Long) As Boolean
    'Dim m As Annunci.AnnunciManager = New AnnunciManager(mConnection)

    'Return _mercatinoManager.deleteTrattativaLogicByAnnuncio(trattativaId)
    'End Function

    Public Function deleteAnnuncio(ByVal annuncioId As Long, ByVal absoluteServerPath As String) As Boolean
        Dim m As Annunci.AnnunciManager = New AnnunciManager(mConnection)
        Return m.deleteAnnuncio(annuncioId, absoluteServerPath)

        'Return _mercatinoManager.deleteAnnuncio(annuncioId, absoluteServerPath)
    End Function

    Public Function deleteUser(ByVal userId As Long, ByVal absoluteServerPath As String) As Long
        Return _mercatinoManager.deleteUser(userId, absoluteServerPath)
    End Function


    Shared Function decodeSourceId(value As String) As String
        Select Case value
            Case "1"
                Return "RevoAgent"
            Case Else
                Throw New MyManager.ManagerException("decodeSourceId: " & value)
        End Select
    End Function


End Class
