#!/usr/bin/env sh
set -e

TEMPLATE="/usr/share/nginx/html/config.template.json"
TARGET="/usr/share/nginx/html/config.json"

if [ -f "$TEMPLATE" ]; then
  if [ -z "$BACKEND_URL" ]; then
    echo "WARN: BACKEND_URL is not set. Using config.template.json as config.json without substitution."
    cp "$TEMPLATE" "$TARGET"
  else
    echo "INFO: Generating config.json from config.template.json using BACKEND_URL=$BACKEND_URL"
    envsubst '${BACKEND_URL}' < "$TEMPLATE" > "$TARGET"
  fi
else
  echo "WARN: config.template.json not found. Skipping runtime config generation."
fi

exec "$@"