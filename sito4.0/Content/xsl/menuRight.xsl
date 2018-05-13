<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output  encoding="ISO-8859-1" indent="yes" omit-xml-declaration="yes"/>

    <xsl:param name="menuLeftSelected">-1</xsl:param>
    <xsl:param name="menuTopSelected">-1</xsl:param>
    <xsl:param name="roles"/> 
    
    

    <xsl:template match="ROOT">
    
   <!-- <xsl:if test="TITLE!=''">
		<div id='menuLeft'><ul><li class="titolo"><span><b><xsl:value-of select="TITLE"/></b></span></li></ul></div><br/>
    </xsl:if>-->
			<xsl:apply-templates select="MENU"/>
    </xsl:template>
    
    <xsl:template match="MENU">
      <div id="menuRight">
        <ul class="menuRight">
          <xsl:if test="TITLE!=''">
            <li class="titolo">
              <span>
                <xsl:value-of select="TITLE"/>
              </span>
            </li>
          </xsl:if>
          <xsl:apply-templates select="./SUBMENU[count(roles/role[contains($roles,.)])&gt;0 or roles/role='*' ]"/>
        </ul>
      </div>
    </xsl:template>    
    
    
    <xsl:template match="SUBMENU">
        <xsl:choose>
			<xsl:when test="value!=''">
			
			<xsl:element name="li">
			
  <xsl:if test="position() = $menuLeftSelected">
            <xsl:attribute name="class">menuLeftSelected</xsl:attribute>
          </xsl:if>
     <!--
      <xsl:if test="position() != $menuLeftSelected">
				<xsl:attribute name="class">menu</xsl:attribute>
				<xsl:attribute name="OnMouseOver">this.className='menuOnMouseOver'</xsl:attribute>
				<xsl:attribute name="OnMouseOut">this.className='menu'</xsl:attribute>
			</xsl:if>
			-->	
			
		
				<xsl:element name="a">

          <xsl:if test="position() = $menuLeftSelected">
            <xsl:attribute name="class">menuLeftSelected</xsl:attribute>
          </xsl:if>
          
					        <xsl:attribute name="target"><xsl:value-of select="target"/></xsl:attribute>
                  <xsl:attribute name="alt"><xsl:value-of select="tip"/></xsl:attribute>
					        <xsl:attribute name="title"><xsl:value-of select="tip"/></xsl:attribute>

       


          <!-- Target Popup -->
                    <xsl:if test="value='Popup'">
                        <xsl:attribute name="href">#</xsl:attribute>
                        <xsl:attribute name="onclick">window.open('<xsl:value-of select="$contesto"/>/<xsl:value-of select="href"/>','<xsl:value-of select="name"/>','height=<xsl:value-of select="height"/>, left=<xsl:value-of select="left"/>, width=<xsl:value-of select="width"/>, top=<xsl:value-of select="top"/>, scrollbars=<xsl:value-of select="scroll"/>');</xsl:attribute>
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
                          <!-- ?menuTopSelected=<xsl:value-of select="$menuTopSelected"/>&amp;menuLeftSelected=<xsl:value-of select="position()"/>-->
                          <xsl:attribute name="href">
                            <xsl:value-of select="href"/>
<!--
                            <xsl:choose>
                              <xsl:when test="contains(href, '?')">
                                <xsl:value-of select="href"/>&amp;menuTopSelected=<xsl:value-of select="$menuTopSelected"/>&amp;menuLeftSelected=<xsl:value-of select="position()"/>
                              </xsl:when>
                              <xsl:otherwise>
                                <xsl:value-of select="href"/>?menuTopSelected=<xsl:value-of select="$menuTopSelected"/>&amp;menuLeftSelected=<xsl:value-of select="position()"/>
                              </xsl:otherwise>
                            </xsl:choose>
-->

                 

                          </xsl:attribute>
                        </xsl:otherwise>
                    </xsl:choose>
                    
                    <xsl:value-of select="value"/>
                </xsl:element>
             </xsl:element>
			<!--</li>-->
			</xsl:when>
          <xsl:otherwise>
			<!-- inserisco uno spazio 
			<li class="empty"></li>	-->
          </xsl:otherwise>
      </xsl:choose>
    </xsl:template>
  
  
</xsl:stylesheet>








