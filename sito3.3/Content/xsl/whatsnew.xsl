<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output omit-xml-declaration ="yes"/>
  <xsl:template match="release">
    <h2>Release <xsl:value-of select="@number"/> -  <xsl:value-of select="@data"/></h2>


    <xsl:if test="count(item[@type='changes'])> 0">
        <h3>Changes</h3>
        <ul>
          <xsl:apply-templates select="item[@type='changes']"></xsl:apply-templates>
        </ul>
   </xsl:if>


    <xsl:if test="count(item[@type='bug'])> 0">
      <h3>Bugs</h3>
      <ul>
        <xsl:apply-templates select="item[@type='bug']"></xsl:apply-templates>
      </ul>
    </xsl:if>
    
  </xsl:template>



  <xsl:template match="item">
    <li><xsl:value-of select="."/></li>
  </xsl:template>
    
    
    
  
  
</xsl:stylesheet>

  
  
  