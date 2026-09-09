export class ApiService {

    #buildHeaders(extraHeaders = {}) {
        return Object.assign({}, extraHeaders)
    }

    #makeRequest(method, url, data = null, extraHeaders = {}, expectedDataType) {  //expectedDataType = json/xml/html/text
        const options = {
            type: method,
            url: url,
            headers: extraHeaders,
            dataType: expectedDataType,
            //dataFilter : - will be configured later, after some research.
        };

        if (method == 'GET' && data) {
            options.data = data;  //serialized as query string
        }

        if (method == 'POST' && data) {
            // MAGIC CHECK: Is this a file upload?
            if (data instanceof FormData) {
                options.processData = false; // Tell jQuery NOT to convert this to a query string
                options.contentType = false; // Tell jQuery NOT to set a content-type (the browser will auto-set 'multipart/form-data' with the correct boundary)
                options.data = data;         // Pass the raw FormData object
            }
            else {
                // Standard JSON POST
                options.contentType = 'application/json; charset=utf-8';
                options.data = JSON.stringify(data);
            }
        }

        return $.ajax(options);
    }

    get(url, data = null, extraHeaders = {}, expectedDataType) {
        return this.#makeRequest("GET", url, data, extraHeaders, expectedDataType);
    }

    post(url, data = null, extraHeaders = {}, expectedDataType) {
        return this.#makeRequest("POST", url, data, extraHeaders, expectedDataType);
    }
}