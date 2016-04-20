<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output  encoding="utf-8" indent="yes" omit-xml-declaration="yes"/>

  <xsl:param name="withSommario">false</xsl:param>


  <xsl:template match="/root">



      <!-- Menu delle categorie-->
      <ul>
        <xsl:apply-templates  select="categoria"  />
      </ul>


      <br />
      <br />
     
    <div id="faq">
      <!-- Elenco FAQ divise per categoria-->
      <xsl:for-each select="categoria">

        <!-- bookmark alla categoria-->
        
        <xsl:element name="a">
          <xsl:attribute name="name">
            <xsl:value-of select="@nome"/>
          </xsl:attribute>
          <h3><xsl:value-of select="@nome"/></h3>
        </xsl:element>

        <ul>
        <xsl:apply-templates  select="faq"  />
        </ul>
    </xsl:for-each>

      </div>



  </xsl:template>







  <xsl:template  match="categoria" >
    <li>

      <!-- link al bookmark-->
      <xsl:element name ="a">
        <xsl:attribute name ="href">
          #<xsl:value-of select="@nome"/>
        </xsl:attribute>
        <xsl:value-of select="@nome"/>
      </xsl:element>

    </li>
  </xsl:template>





  <xsl:template match="faq">
    <li>
      <p>
        <spam style="color:red;">Domanda</spam>: <xsl:value-of select="domanda"/>
      </p>
      <p>
        <spam style="color:blue;">Risposta</spam>: <xsl:value-of select="risposta"/>
      </p>
    </li>
  </xsl:template>


</xsl:stylesheet>


