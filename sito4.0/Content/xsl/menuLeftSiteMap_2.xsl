<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output encoding="ISO-8859-1" indent="yes" omit-xml-declaration="yes"/>
  <!-- Parametri -->
  <xsl:param name="urlCorrente"/>
  <xsl:param name="roles"/>
  <xsl:param name="contesto"/>


  <xsl:template match="siteMap">
    <div class="menuLeft_3">
      <ul class="menuLeft_3">


        <!-- -->
        <xsl:if test="string-length(siteMapNode/siteMapNode[contains(@url,$urlCorrente)]/@DescriptionMenuTop) &gt; 0">
          <xsl:apply-templates select="siteMapNode/siteMapNode[contains(@url,$urlCorrente)]" mode="LEVEL_1"/>
        </xsl:if>


        <xsl:if test="string-length(siteMapNode/siteMapNode[contains(@url,$urlCorrente)]/@DescriptionMenuTop) = 0">
          <xsl:apply-templates select="siteMapNode/siteMapNode[siteMapNode[contains(@url,$urlCorrente)]]" mode="LEVEL_1"/>
        </xsl:if>


      </ul>
    </div>
  </xsl:template>




  <xsl:template match="siteMapNode" mode="LEVEL_1">

    <!--- nodo padre -->
    <xsl:if test="string-length(@DescriptionMenuTop) &gt; 0">
      <!--  <xsl:variable name="Codes" select="@authorization"/>
       <xsl:if test="contains($roles, '-12345') or ((contains($roles, $Codes) or (@authorization='?' and string-length($roles) &gt; 0) or @authorization='*'))">
      <xsl:if test="contains($roles, '-12345') or (contains($roles, @authorization) )">-->
      <xsl:element name="li">
        <xsl:element name="a">
          <xsl:attribute name="href">
            <xsl:value-of select="$contesto"/>
            <xsl:value-of select="substring(@url,2)"/>
          </xsl:attribute>
          <xsl:value-of select="@title"/>
        </xsl:element>
      </xsl:element>
      <!-- </xsl:if>-->


      <xsl:apply-templates select="./siteMapNode" mode ="LEVEL_2"/>


    </xsl:if>










  </xsl:template>





  <xsl:template match="siteMapNode" mode="LEVEL_2">

    <xsl:if test="@hide ='false' ">

      <xsl:element name="li">
        <xsl:element name="a">
          <xsl:attribute name="href">
            <xsl:value-of select="$contesto"/>
            <xsl:value-of select="substring(@url,2)"/>
          </xsl:attribute>
          <xsl:value-of select="@title"/>
        </xsl:element>
      </xsl:element>

    </xsl:if>

  </xsl:template>










</xsl:stylesheet>
