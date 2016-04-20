<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output  encoding="utf-8" indent="yes" omit-xml-declaration="yes"/>

<xsl:param name="withSommario">false</xsl:param>


	<xsl:template match="/root">


    <xsl:if test="$withSommario='true'">
		<xsl:apply-templates  mode="sommario" select="faq" />
	</xsl:if>

    
		<div id="faq">    
		<ul>
      <xsl:apply-templates  select="faq"  />
		</ul>
	</div>
	</xsl:template>
    
    
    
        
        
    <xsl:template  match="faq" mode="sommario" >
		bb<li>Domanda: <xsl:value-of select="domanda"/></li>
    </xsl:template>
    
    
    <xsl:template match="faq">
		<li>
			<p>
        <spam style="color:red;">Domanda</spam>: <xsl:value-of select="domanda"/> </p>
			<p>
        <spam style="color:blue;">Risposta</spam>: <xsl:value-of select="risposta"/> </p>
		</li>
        </xsl:template>
        
        
</xsl:stylesheet>


  