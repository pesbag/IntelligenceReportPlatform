#!/bin/sh

until curl -s http://elasticsearch:9200/_cluster/health | grep -q '"status"'; do
  sleep 2
done

curl -X PUT "http://elasticsearch:9200/reports-logs" \
  -H "Content-Type: application/json" \
  -d '{
    "mappings": {
      "properties": {
        "reportId":    { "type": "keyword" },
        "timestamp":   { "type": "date" },
        "agentId":     { "type": "keyword" },
        "unit":        { "type": "keyword" },
        "theater":     { "type": "keyword" },
        "sector":      { "type": "keyword" },
        "location":    { "type": "keyword" },
        "reportType":  { "type": "keyword" },
        "priority":    { "type": "keyword" },
        "sourceType":  { "type": "keyword" },
        "message":     { "type": "text" },
        "subjectId":   { "type": "keyword" },
        "subjectType": { "type": "keyword" },
	      "processedAt": {"type":"date"}
      }
    }
  }'