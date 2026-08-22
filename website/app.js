/* -------------------------------------------------------------
   IDEMIA Morpho L1 RD Service AI Suite - Simulator Logic
   Lead Architect: Raj Kishor Mahapatra (https://itsrkmahapatra.qzz.io/)
------------------------------------------------------------- */

document.addEventListener('DOMContentLoaded', () => {
  const btnSimRd = document.getElementById('btnSimRd');
  const btnSimDev = document.getElementById('btnSimDev');
  const btnSimCap = document.getElementById('btnSimCap');
  const btnCopyXml = document.getElementById('btnCopyXml');
  const btnClearXml = document.getElementById('btnClearXml');
  
  const simXmlOutput = document.getElementById('simXmlOutput');
  const simStatusText = document.getElementById('simStatusText');
  const simStatusPill = document.getElementById('simStatusPill');
  const simScoreBadge = document.getElementById('simScoreBadge');

  const simEnv = document.getElementById('simEnv');
  const simFormat = document.getElementById('simFormat');
  const simFCount = document.getElementById('simFCount');
  const simFType = document.getElementById('simFType');
  const simTimeout = document.getElementById('simTimeout');

  function setStatus(text, color, badgeText = 'Active') {
    simStatusText.textContent = text;
    simStatusText.style.color = color;
    simStatusPill.style.borderColor = color;
    simScoreBadge.textContent = badgeText;
  }

  // 1. RDSERVICE Simulation
  btnSimRd.addEventListener('click', async () => {
    setStatus('DISCOVERING RD SERVICE (11100)...', '#F59E0B', 'Pinging...');
    simXmlOutput.innerHTML = `<code><!-- Querying https://127.0.0.1:11100/ over TLS 1.2 Socket... --></code>`;

    await new Promise(r => setTimeout(r, 450));

    const rdXml = `<?xml version="1.0"?>
<RDService status="READY" info="IDEMIA_L1_RDSERVICE">
  <Interface id="CAPTURE" path="/127.0.0.1:11100/capture"/>
  <Interface id="DEVICEINFO" path="/127.0.0.1:11100/getDeviceInfo"/>
</RDService>`;

    simXmlOutput.innerHTML = `<code>${escapeHtml(rdXml)}</code>`;
    setStatus('✔ RD SERVICE READY (IDEMIA L1)', '#10B981', 'Status: READY');
  });

  // 2. DEVICEINFO Simulation
  btnSimDev.addEventListener('click', async () => {
    setStatus('QUERYING DEVICE INFO...', '#F59E0B', 'Querying...');
    simXmlOutput.innerHTML = `<code><!-- Querying https://127.0.0.1:11100/getDeviceInfo over TLS 1.2 Socket... --></code>`;

    await new Promise(r => setTimeout(r, 550));

    const devXml = `<?xml version="1.0"?>
<DeviceInfo dpId="Morpho.SmartChip" rdsId="NXP.IDEMIA.001" rdsVer="1.1.4" dc="dbf58ad9-4913-42ba-83e9-dedfa9e5512a" mi="CBME3RD" mc="MIIEIzCCAwugAwIBAgIQYSXAA9FtsGS1XgIyK5EcTzANBgkqhkiG9w0BAQsFADCB8DEgMB4GA1UEAxMXRFMgU01BUlQgQ0hJUCBQVlQgTFREIDUxSTBHBgNVBDMTQDEtQSwgU0VDVE9SLTczLCBOT0lEQSwgR2F1dGFtYnVkZGhhIE5hZ2FyLCBVdHRhciBQcmFkZXNoLCAyMDEzMDcxHDAaBgNVBAkTE0dBVVRBTSBCVURESEEgTkFHQVIxFjAUBgNVBAgTDVVUVEFSIFBSQURFU0gxITAfBgNVBAsTGERJR0lUQUwgU0VSVklDRSBERUxJVkVSWTEbMBkGA1UEChMSU01BUlQgQ0hJUCBQVlQgTFREMQswCQYDVQQGEwJJTjAeFw0yNjA4MjIwODIzNDVaFw0yNjA5MjEwODIzNDVaMG8xCzAJBgNVBAYMAklOMQswCQYDVQQIDAJVUDEOMAwGA1UEBwwFTm9pZGExDzANBgNVBAoMBk1vcnBobzEPMA0GA1UECwwGTW9ycGhvMSEwHwYDVQQDDBgwMDAyOTM3NDI1MDYtMjQzOEkwNTA2NzQwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDywQR1JQXXpDxtGC645N7UvnImwnWDMvLx2z17Xucg9a8JoX5Se8Ud7yjPbLijM7xnogN5tUxoP7V1Z0ql6fNOWZVkeC7qvkt6eHkqoXC5qT6+2p7HdRs1+gIQOjogGsmHy8ttm2fU6JLE5Dga2f0/Q9Z87g3BIseOv3ropDilcwNgN8iNuPARKkp2kjVw8lN38IqYYlBKV5F2qEPbwiG57o1xhFAN6e0ZwoeTEcnMczBbGOv7sMGvT28jYhQx8yhJTI5zTZaHqj2PA7kAsoV9jQkGlC03UfmWMSNr2psyjGhb+Z0g8jsfoXZ0DYiDnNOZxvAS17mucfyRa4oXvYVxAgMBAAGjOTA3MB0GA1UdDgQWBBTju4mkXy9nTCjiqKkW/Zfs3G0q7jAJBgNVHRMEAjAAMAsGA1UdDwQEAwIGQDANBgkqhkiG9w0BAQsFAAOCAQEAHwMaJhv0Mus974z4J3DETYcotrzFfzZsUdJvN0wuHOaRFJNDeFH97F9BcTFb7VUs2AqwnytLDxYMhJ2U3BBKZeXucnSfvl7114wINtVvSiF7mvuoN1Pq88Wi5LyeSeg6DOvT5vFY2SY92LGa+dH5b4O8SMBrLIc5nS/ynvVF/0H9cziaUVsNWLLoYsRqLFM9bGLJoL6JNlI4QsQgYPz5++kwGLK52Oxtm29k4kwkycRoyMltXPFeYJN7yI9aPkm/tzYmcgFIjVn/9+NmeBcNp9nVvN8fWGTCKLAUQ3tPBnvvUaF/luNlhf94VSlZ37MyMSAgomoIoTidRQj2GR8n3A==">
  <additional_info>
    <Param name="srno" value="2438I050674"/>
    <Param name="serial_number" value="2438I050674"/>
    <Param name="modality_type" value="Finger"/>
    <Param name="device_type" value="L1"/>
  </additional_info>
</DeviceInfo>`;

    simXmlOutput.innerHTML = `<code>${escapeHtml(devXml)}</code>`;
    setStatus('✔ DEVICE ONLINE (S/N: 2438I050674)', '#10B981', 'S/N: 2438I050674');
  });

  // 3. CAPTURE Simulation
  btnSimCap.addEventListener('click', async () => {
    setStatus('🔴 SCANNER ACTIVE — SENSING FINGER...', '#EF4444', 'Scanning...');
    simXmlOutput.innerHTML = `<code><!-- Activating Morpho MSO 1300 E3 L1 Optical Sensor (LED Active)...
     Waiting for biometric touch... --></code>`;

    await new Promise(r => setTimeout(r, 1200));

    const env = simEnv.value;
    const format = simFormat.value;
    const fCount = simFCount.value || '1';
    const fType = simFType.value || '0';
    const score = Math.floor(Math.random() * 16) + 84; // 84% - 99%

    const capXml = `<?xml version="1.0"?>
<PidData>
  <Resp errCode="0" errInfo="" fCount="${fCount}" fType="${fType}" iCount="0" iType="0" pCount="0" pType="0" nmPoints="49" qScore="${score}"/>
  <DeviceInfo dpId="Morpho.SmartChip" rdsId="NXP.IDEMIA.001" rdsVer="1.1.4" dc="dbf58ad9-4913-42ba-83e9-dedfa9e5512a" mi="CBME3RD" mc="..."/>
  <Skey ci="20280813">LS0m4fPQ2WEOiRPnW+jXXnDqJhtsEGHuYggrI9ftMnBMJYCpTU4F5r8YV6V1ThEpK47nykCmz8pU4RIqHPh4KVk2NXxbZHPskL6AMw7hxcu6uj92V0PQGBRptTRrkH8UEvBHOcQrl7vHvpWLSeq3xL9imRoeUzSjH+lwdyDfzODkY2Syw7pl4L6LAGFiZEp1DnRodBhukI4/yPiNRL/gw19plTovSn2DTr94cxkiClzxzRIWHt0D0xDK0xo9eYUTwgZzsQp+W1E5GuTKVwoE3axnS0Ktas+04Q4Vcf6WabQ6ZXz6o6sj13NPAg5kSGbpfCCBRMJTULlyWVAP7FIa9A==</Skey>
  <Hmac>ZCGJvjjr7zFZozssHEc5uUh9Z0D3Iphwg46L3i/2tLhrTt6KehsiYmLliL3xJ/v5</Hmac>
  <Data type="X">MjAyNi0wOC0yMlQxNDowMzo1MgL/rJtpoV/PEt798zvztg0xwvBjw8LqEXgWmyKp1VICmETKFoMmjsd42o16YCvnYgoCNN1q8r5Ob2sYmhziHRPyx2HNrtR+XdJ2UkTX8hG/84uR0jJ28Ap1Dj6dciNG2LEymjVZrVdsu1XztS+GUiYV3OvxVE0aun3x9VHb0k9a5X/iIUY3xGztWz+/ev6okI0FlikuSN4HwtyGC7lH3XRVkibJa97ECvxdb8AiAWdpetd2R6Uf2KqWXWPXkxfQvLNvpo1nuGUUNPmM0UzL08/uk46BtB/66kxAtEcXVWWkRdkw8uxhuGM2B9ZElvG6+eqpxpYYBluMHH7JyYxPY9kFHfOvawDDxuPn8Od5uLjUBNVG6qMFzd2bI+tqUXz1R/tE6UzODE+FUqBdbRdA25m2GQgomBwOtPxP6eDBLpLhdjOU238he1SJKLRDJV+Y3RA9MKQbbua9zBVLiRPuUegNY4rxxmp17Jy6T8BxmVopeXxTCpRkkGJdoWwhuQd+czj67X5HAr/TaAfdVxTOFc63e9G3XO+RVQC6r/JOisbuLwDrRSNxqjQvVkh1QjViKIQHL2wh93a/brDaCiiTuvX6qCWs1VDFSjz5mXae3amwKhng9WNXLe0Ay7OyAg9ZH+NdMBJy4hz5m7dwap5buz2VTp/3PhomKq1g6YkdR4dfyKBOZk4OY/uRnYyEeMarW+s9/33sPrHCn3Sr85S6pe1wrQwYX8hionQVBADVoKemkIfA0k8Jw4kkTGgGs1NBlb91pFAkVmgJ5v6yi2OjxHjsgiQ8z14/eBqPMcP1yuSkexTTfsCAoy8zMNpd70aza6+eho23Sfn6DrgC5utRPN7WChEFOYHi/6vfMfqOhDlQAPr0NhI/4pn9LRaIvqCNd1SAz5QcABCU93TNty7ie9PM5kK+kzd6rZZj5nvjc4pqZTzZhJzrSROdq93Uu0iR3fA75HE4TUU4EGNoTew33gQLFISHedJ7fafr0EC6x5f/9WppOAsOdb3N46KHFLPKCtB0agKAHz1sn5r0VO1mUSrqhpNffjOiEqkfaE1FMDE1kpqcFAiZMWycRO5Fst+Io7Czx+DmGEiZ1v+jKoZmJ3hnVupKOd/rr2WTj348OMpFf4WrjEf3u8pA8TNiSAyqFW3GejDA+kOxSQq1KbybzpS381qU8wChQEoLpScDwdb6LW9vBvNqPJYVqhkm0LkSaDw/L0xV0XU8ptrebgOJ9qaFn3wMhowT6tpwxqsfnYp06cTLgHTIO2wwTPmAlhQg+xDbLv/quAWFB4/AmGj0TXR133dnv6Vfq2y78VnvZKsHhkruVxDQ9xi/OeavDViSMke6mxTRsGL57iim/Yr5IZRWxHP09WWxyAQKKef1Fqj7pBpj69yOFJWnylLNSirxoCpW/GihEh0/HxWfGwxuF2h4f2iA0pac+7oVUNW3OgTAGIGt</Data>
</PidData>`;

    simXmlOutput.innerHTML = `<code>${escapeHtml(capXml)}</code>`;
    setStatus(`✔ CAPTURE SUCCESSFUL (Quality: ${score}%)`, '#10B981', `Quality: ${score}%`);
  });

  // 4. Copy XML
  btnCopyXml.addEventListener('click', () => {
    const rawText = simXmlOutput.innerText;
    navigator.clipboard.writeText(rawText).then(() => {
      const orig = btnCopyXml.innerHTML;
      btnCopyXml.innerHTML = '<span>✔</span> Copied!';
      setTimeout(() => { btnCopyXml.innerHTML = orig; }, 1500);
    });
  });

  // 5. Clear XML
  btnClearXml.addEventListener('click', () => {
    simXmlOutput.innerHTML = `<code>&lt;!-- Console Cleared. Ready for new RD request. --&gt;</code>`;
    setStatus('SCANNER STATUS: IDLE', '#94A3B8', 'Ready');
  });

  function escapeHtml(text) {
    return text
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;")
      .replace(/'/g, "&#039;");
  }
});
