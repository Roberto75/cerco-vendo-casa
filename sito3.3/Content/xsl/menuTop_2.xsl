<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output encoding="ISO-8859-1" indent="yes" omit-xml-declaration="yes"/>
	<!-- Parametri -->
	<xsl:param name="menuTopSelected">-1</xsl:param>
	<xsl:param name="roles"/>
	<xsl:param name="contesto"/>
	<xsl:template match="ROOT">
    <div id="menuTop_2">
      <ul>
        <!--li>Roles:<xsl:value-of select="$roles"/></li-->
        <xsl:apply-templates select="./MENU[count(roles/role[contains($roles,.)])&gt;0 or roles/role='*' ]"/>
        <!--li><img src="images/tab_right.gif"></img></li-->
      </ul>
    </div>
	</xsl:template>
	<!--    
    <xsl:template match="MENU[id=0]">
		<li class="menu0">
			<xsl:attribute name="class"><xsl:value-of select="class"/></xsl:attribute>
			<p>				
				<xsl:element name="a">
					<xsl:attribute name="href"><xsl:value-of select="href"/></xsl:attribute>
					<xsl:value-of select="value"/>
				</xsl:element>
			</p>
		</li>
    </xsl:template>
    
    <xsl:template match="MENU[id='fine']">
		<li class="menuend">
			<xsl:attribute name="class"><xsl:value-of select="class"/></xsl:attribute>
			<p><xsl:value-of select="value"/></p>
		</li>
    </xsl:template>
    -->
	<xsl:template match="MENU">
		<xsl:choose>
			<xsl:when test="value!=''">
				<!-- se l'utente e' registrato non faccio visualizzzare il menu con onlyAnonimo=true -->
			  <xsl:if test=" not (contains($roles, 'Authenticated') and  @onlyAnonimo ='true')">
          

          <xsl:element name="li">
						<!--<xsl:attribute name="name"><xsl:value-of select="id"/></xsl:attribute>-->
						
							<xsl:element name="a">
                <xsl:if test="position() = $menuTopSelected">
                  <xsl:attribute name="class">menuTopSelected</xsl:attribute>
                </xsl:if>
             <!--   <xsl:if test="position() != $menuSelected">
                  <xsl:attribute name="class">menu</xsl:attribute>
                  <xsl:attribute name="OnMouseOver">this.className='menuOnMouseOver'</xsl:attribute>
                  <xsl:attribute name="OnMouseOut">this.className='menu'</xsl:attribute>
                </xsl:if>
-->
                <xsl:attribute name="target"><xsl:value-of select="target"/></xsl:attribute>
								<xsl:attribute name="title"><xsl:value-of select="tip"/></xsl:attribute>
								<!-- Target Popup -->
								<xsl:if test="value='Popup'">
									<xsl:attribute name="href">#</xsl:attribute>
									<xsl:attribute name="onclick">window.open('<xsl:value-of select="href"/>','<xsl:value-of select="name"/>','height=<xsl:value-of select="height"/>, left=<xsl:value-of select="left"/>, width=<xsl:value-of select="width"/>, top=<xsl:value-of select="top"/>, scrollbars=<xsl:value-of select="scroll"/>');</xsl:attribute>
								</xsl:if>
								<xsl:choose>
									<xsl:when test="href=''">
										<!-- Apre il sottomenu -->
										<xsl:attribute name="href">#</xsl:attribute>
										<!--<xsl:attribute name="onclick">Apri_Chiudi('<xsl:value-of select="$menuParent"/>_');</xsl:attribute>-->
									</xsl:when>
									<xsl:when test="starts-with(href,'javascript')">
										<xsl:attribute name="href">#</xsl:attribute>
										<!--<xsl:attribute name="onclick"><xsl:value-of select="href"/>;apri('<xsl:value-of select="$menuParent"/>_');</xsl:attribute>-->
										<xsl:attribute name="onclick"><xsl:value-of select="href"/></xsl:attribute>
									</xsl:when>
									<xsl:otherwise>
										<!-- Rimanda all'Url -->
										<xsl:attribute name="href"><xsl:value-of select="$contesto"/>/<xsl:value-of select="href"/>?menuTopSelected=<xsl:value-of select="position()"/>&amp;menuLeftSelected=1
                  </xsl:attribute>
									</xsl:otherwise>
								</xsl:choose>
								<xsl:value-of select="value"/>
							</xsl:element>
						
						<!--</li>-->
					</xsl:element>
				</xsl:if>
			</xsl:when>
			<xsl:otherwise>
				<!-- inserisco uno spazio 
			<li class="empty"></li>	-->
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>
