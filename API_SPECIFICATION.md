# UIDAI Aadhaar L1 Registered Device API Specification

## 1. Endpoints & Verbs

The UIDAI Registered Device specification requires local HTTP/HTTPS services listening on `127.0.0.1` (ports 11100 to 11120).

| Method (HTTP Verb) | Endpoint | Description | Request Body | Response Format |
| :--- | :--- | :--- | :--- | :--- |
| `RDSERVICE` | `https://127.0.0.1:11100/` | Discovery & Service Readiness | None | XML (`<RDService>`) |
| `DEVICEINFO` | `https://127.0.0.1:11100/getDeviceInfo` | Hardware Metadata & Serial Number | None | XML (`<DeviceInfo>`) |
| `CAPTURE` | `https://127.0.0.1:11100/capture` | Biometric Fingerprint Capture | XML (`<PidOptions>`) | XML (`<PidData>`) |

---

## 2. Request & Response Formats

### A. Discovery (`RDSERVICE`)
**Request:**
```http
RDSERVICE / HTTP/1.1
Host: 127.0.0.1:11100
User-Agent: Morpho_Studio
Accept: text/xml
```

**Response:**
```xml
<?xml version="1.0"?>
<RDService status="READY" info="IDEMIA_L1_RDSERVICE">
  <Interface id="CAPTURE" path="/127.0.0.1:11100/capture"/>
  <Interface id="DEVICEINFO" path="/127.0.0.1:11100/getDeviceInfo"/>
</RDService>
```

---

### B. Device Info (`DEVICEINFO`)
**Request:**
```http
DEVICEINFO /getDeviceInfo HTTP/1.1
Host: 127.0.0.1:11100
Accept: text/xml
```

**Response:**
```xml
<?xml version="1.0"?>
<DeviceInfo dpId="Morpho.SmartChip" rdsId="NXP.IDEMIA.001" rdsVer="1.1.4" dc="dbf58ad9-4913-42ba-83e9-dedfa9e5512a" mi="CBME3RD" mc="MIIEIzCCAwugAwIBAgIQYSXAA9FtsGS1XgIyK5EcTzANBgkqhkiG9w0BAQsFADCB...">
  <additional_info>
    <Param name="srno" value="2438I050674"/>
    <Param name="serial_number" value="2438I050674"/>
    <Param name="modality_type" value="Finger"/>
    <Param name="device_type" value="L1"/>
  </additional_info>
</DeviceInfo>
```

---

### C. Biometric Capture (`CAPTURE`)
**Request (`PidOptions`):**
```xml
<PidOptions ver="1.0">
  <Opts env="P" fCount="1" fType="0" format="0" pidVer="2.0" timeout="10000" otp="" wadh="" posh=""/>
</PidOptions>
```

**Response (`PidData`):**
```xml
<?xml version="1.0"?>
<PidData>
  <Resp errCode="0" errInfo="" fCount="1" fType="0" iCount="0" iType="0" pCount="0" pType="0" nmPoints="49" qScore="89"/>
  <DeviceInfo dpId="Morpho.SmartChip" rdsId="NXP.IDEMIA.001" rdsVer="1.1.4" dc="dbf58ad9-4913-42ba-83e9-dedfa9e5512a" mi="CBME3RD" mc="..."/>
  <Skey ci="20280813">LS0m4fPQ2WEOiRPnW+jXXnDqJhtsEGHuYggrI9ftMnBMJYCpTU4F5r8YV6V1ThEpK47nykCmz8pU4RIqHPh4KVk2NXxbZHPskL6AMw7hxcu6uj92V0PQGBRptTRrkH8UEvBHOcQrl7vHvpWLSeq3xL9imRoeUzSjH+lwdyDfzODkY2Syw7pl4L6LAGFiZEp1DnRodBhukI4/yPiNRL/gw19plTovSn2DTr94cxkiClzxzRIWHt0D0xDK0xo9eYUTwgZzsQp+W1E5GuTKVwoE3axnS0Ktas+04Q4Vcf6WabQ6ZXz6o6sj13NPAg5kSGbpfCCBRMJTULlyWVAP7FIa9A==</Skey>
  <Hmac>ZCGJvjjr7zFZozssHEc5uUh9Z0D3Iphwg46L3i/2tLhrTt6KehsiYmLliL3xJ/v5</Hmac>
  <Data type="X">MjAyNi0wOC0yMlQxNDowMzo1MgL/rJtpoV/PEt798zvztg0xwvBjw8LqEXgWmyKp1VICmETKFoMmjsd42o16YCvnYgoCNN1q8r5Ob2sYmhziHRPyx2HNrtR+XdJ2UkTX8hG/84uR0jJ28Ap1Dj6dciNG2LEymjVZrVdsu1XztS+GUiYV3OvxVE0aun3x9VHb0k9a5X/iIUY3xGztWz+/ev6okI0FlikuSN4HwtyGC7lH3XRVkibJa97ECvxdb8AiAWdpetd2R6Uf2KqWXWPXkxfQvLNvpo1nuGUUNPmM0UzL08/uk46BtB/66kxAtEcXVWWkRdkw8uxhuGM2B9ZElvG6+eqpxpYYBluMHH7JyYxPY9kFHfOvawDDxuPn8Od5uLjUBNVG6qMFzd2bI+tqUXz1R/tE6UzODE+FUqBdbRdA25m2GQgomBwOtPxP6eDBLpLhdjOU238he1SJKLRDJV+Y3RA9MKQbbua9zBVLiRPuUegNY4rxxmp17Jy6T8BxmVopeXxTCpRkkGJdoWwhuQd+czj67X5HAr/TaAfdVxTOFc63e9G3XO+RVQC6r/JOisbuLwDrRSNxqjQvVkh1QjViKIQHL2wh93a/brDaCiiTuvX6qCWs1VDFSjz5mXae3amwKhng9WNXLe0Ay7OyAg9ZH+NdMBJy4hz5m7dwap5buz2VTp/3PhomKq1g6YkdR4dfyKBOZk4OY/uRnYyEeMarW+s9/33sPrHCn3Sr85S6pe1wrQwYX8hionQVBADVoKemkIfA0k8Jw4kkTGgGs1NBlb91pFAkVmgJ5v6yi2OjxHjsgiQ8z14/eBqPMcP1yuSkexTTfsCAoy8zMNpd70aza6+eho23Sfn6DrgC5utRPN7WChEFOYHi/6vfMfqOhDlQAPr0NhI/4pn9LRaIvqCNd1SAz5QcABCU93TNty7ie9PM5kK+kzd6rZZj5nvjc4pqZTzZhJzrSROdq93Uu0iR3fA75HE4TUU4EGNoTew33gQLFISHedJ7fafr0EC6x5f/9WppOAsOdb3N46KHFLPKCtB0agKAHz1sn5r0VO1mUSrqhpNffjOiEqkfaE1FMDE1kpqcFAiZMWycRO5Fst+Io7Czx+DmGEiZ1v+jKoZmJ3hnVupKOd/rr2WTj348OMpFf4WrjEf3u8pA8TNiSAyqFW3GejDA+kOxSQq1KbybzpS381qU8wChQEoLpScDwdb6LW9vBvNqPJYVqhkm0LkSaDw/L0xV0XU8ptrebgOJ9qaFn3wMhowT6tpwxqsfnYp06cTLgHTIO2wwTPmAlhQg+xDbLv/quAWFB4/AmGj0TXR133dnv6Vfq2y78VnvZKsHhkruVxDQ9xi/OeavDViSMke6mxTRsGL57iim/Yr5IZRWxHP09WWxyAQKKef1Fqj7pBpj69yOFJWnylLNSirxoCpW/GihEh0/HxWfGwxuF2h4f2iA0pac+7oVUNW3OgTAGIGt</Data>
</PidData>
```

---

## 3. Standard Error Codes

| errCode | Meaning | Recommended Action |
| :--- | :--- | :--- |
| `0` | **Success** | Biometric captured and encrypted cleanly |
| `700` | Capture Timeout | Finger not placed before timeout expired |
| `710` | Bad Quality / Low Score | Place finger firmly and centrally on glass |
| `720` | Device Not Ready / Blocked | Check USB connection or disable HVCI (Memory Integrity) |
| `-1` | Connection Refused / Offline | Start `Idemia_L1RD_Service` and `MSO_SpUsb_Service` |
