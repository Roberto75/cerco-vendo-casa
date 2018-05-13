<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output encoding="ISO-8859-1" indent="yes" omit-xml-declaration="yes"/>
  <!-- Parametri -->
  <xsl:param name="menuTopSelected">-1</xsl:param>
  <xsl:param name="roles"/>
  <xsl:param name="contesto"/>
  <xsl:template match="siteMap">
    <div class="menuTop_3">
      <ul class="menuTop_3" cellpadding="0" cellspacing="0" border="0">

        <xsl:apply-templates select="siteMapNode/siteMapNode"/>

      </ul>
    </div>
  </xsl:template>




  <xsl:template match="siteMapNode">
    <xsl:if test="string-length(@DescriptionMenuTop) &gt; 0">
      <!--<xsl:variable name="Codes" select="@authorization"/>
       <xsl:if test="contains($roles, '-12345') or ((contains($roles, $Codes) or (@authorization='?' and string-length($roles) &gt; 0) or @authorization='*'))">
      <xsl:if test="contains($roles, '-12345') or (contains($roles, @authorization) )"> -->

      <!-- Per ora escludo il controllo sullo atorizzazioni-->

      <xsl:element name="li">
        <xsl:if test="position() = $menuTopSelected">
          <xsl:attribute name="class">menuTopSelected</xsl:attribute>
        </xsl:if>
        <xsl:element name="a">
          <xsl:attribute name="href">
            <xsl:value-of select="$contesto"/>
            <xsl:value-of select="substring(@url,2)"/>
          </xsl:attribute>
          <xsl:value-of select="@DescriptionMenuTop"/>
        </xsl:element>
      </xsl:element>
    </xsl:if>
    <!-- </xsl:if> -->
  </xsl:template>



</xsl:stylesheet>
