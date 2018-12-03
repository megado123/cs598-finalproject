<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>


  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>



  <xsl:template match="@*">
    <xsl:variable name="namespace">
      <xsl:choose>
        <xsl:when test="namespace-uri()">
          <xsl:value-of select="namespace-uri()"></xsl:value-of>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="namespace-uri(..)" />

        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:element name="{name()}" namespace="{$namespace}">
      <xsl:value-of select="." />
    </xsl:element>
  </xsl:template>





  

</xsl:stylesheet>
