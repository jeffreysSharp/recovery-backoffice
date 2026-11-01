#!/bin/sh
set -e

# Fail fast on missing required values rather than serve a broken bundle.
: "${Recovery_API_URL:?Recovery_API_URL is required (e.g. https://api.example.com)}"
: "${Recovery_DASHBOARD_URL:?Recovery_DASHBOARD_URL is required (e.g. https://app.example.com)}"

# Defaults for non-required values.
: "${Recovery_DEFAULT_TENANT:=root}"

export Recovery_API_URL Recovery_DASHBOARD_URL Recovery_DEFAULT_TENANT

# Render the runtime config into nginx's web root on every start. The template
# lives outside the web root, so it is never served and survives a restart.
envsubst < /etc/recovery/config.json.template > /usr/share/nginx/html/config.json

exec nginx -g 'daemon off;'
