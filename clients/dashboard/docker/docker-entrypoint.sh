#!/bin/sh
set -e

: "${Recovery_API_URL:?Recovery_API_URL is required (e.g. https://api.example.com)}"
: "${Recovery_DEFAULT_TENANT:=root}"

export Recovery_API_URL Recovery_DEFAULT_TENANT

envsubst < /etc/recovery/config.json.template \
       > /usr/share/nginx/html/config.json

exec nginx -g 'daemon off;'
