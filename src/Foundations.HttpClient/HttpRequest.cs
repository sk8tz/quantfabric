﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Foundations.Extensions;
using Foundations.Http;
using Foundations.HttpClient.Authenticators;
using Foundations.HttpClient.Enums;
using Foundations.HttpClient.ParameterHandlers;

namespace Foundations.HttpClient
{
    public class HttpRequest
    {
        private readonly Uri _baseAddress;
        private readonly HttpRequestMessage _message = 
            new HttpRequestMessage();
        private readonly List<KeyValuePair<string, string>> _queryParameters = 
            new List<KeyValuePair<string, string>>();
        private readonly List<KeyValuePair<string, string>> _pathParameters =
            new List<KeyValuePair<string, string>>();

        private string _path;
        private IAuthenticator _authenticator;
        private IParameterHandler _parameterHandler;

        public HttpMethod Method => _message.Method;

        public Uri Url => new Uri(_baseAddress + _path);

        public IEnumerable<KeyValuePair<string, string>> QueryParameters => _queryParameters;

        public HttpRequest(Uri baseAddress)
        {
            _baseAddress = baseAddress;
        }

        public HttpRequest(string baseAddress) : 
            this(new Uri(baseAddress))
        { }

        public HttpRequest Request(HttpMethod method, string path)
        {
            if (method == HttpMethod.Get)
            {
                return GetFrom(path);
            }
            else if (method == HttpMethod.Post)
            {
                return PostTo(path);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public HttpRequest PostTo(string path)
        {
            _path = path.TrimStart('/');

            _message.Method = HttpMethod.Post;
            _parameterHandler = new PostParameterHandler();

            return this;
        }

        public HttpRequest GetFrom(string path)
        {
            _path = path.TrimStart('/');

            _message.Method = HttpMethod.Get;
            _parameterHandler = new GetParameterHandler();

            return this;
        }

        public HttpRequest WithQueryParameters()
        {
            _parameterHandler = new GetParameterHandler();

            return this;
        }

        public HttpRequest Accepts(string contentType)
        {
            _message.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(contentType));

            return this;
        }

        public HttpRequest Accepts(MediaTypeEnum contentType)
        {
            return Accepts(contentType.EnumToString());
        }

        public HttpRequest UserAgent(
            string agent, 
            string version)
        {
            _message.Headers.UserAgent.Add(
                new ProductInfoHeaderValue(
                    agent, 
                    version));

            return this;
        }

        public HttpRequest AcceptsEncoding(string encoding)
        {
            _message.Headers.AcceptEncoding.Add(
                new StringWithQualityHeaderValue(encoding));

            return this;
        }

        public HttpRequest Bearer(string token)
        {
            _message.Headers.Add(
                HttpRequestHeader.Authorization.ToString(),
                $"{OAuth2ParameterEnum.BearerHeader.EnumToString()} {token}");

            return this;
        }

        public HttpRequest Header(HttpRequestHeader header, string value)
        {
            return Headers(new Dictionary<HttpRequestHeader, string>
            {
                {header, value}
            });
        }

        public HttpRequest Headers(Dictionary<HttpRequestHeader, string> headers)
        {
            foreach (var header in headers)
            {
                _message.Headers.Add(
                    header.Key.ToString(), 
                    header.Value);
            }

            return this;
        }

        public HttpRequest Parameter(
            string key, 
            string value)
        {
            _queryParameters.Add(
                new KeyValuePair<string, string>(
                    key, 
                    value));

            return this;
        }

        public HttpRequest Parameters(
            IEnumerable<KeyValuePair<string, string>> parameters)
        {
            foreach (var parameter in parameters)
            {
                _queryParameters.Add(parameter);
            }

            return this;
        }

        public HttpRequest Segments(
            IEnumerable<KeyValuePair<string, string>> parameters)
        {
            foreach (var parameter in parameters)
            {
                _pathParameters.Add(parameter);
            }

            return this;
        }

        public HttpRequest Content(
            string content, 
            MediaTypeEnum mediaType)
        {
            return Content(
                content, 
                mediaType.EnumToString(), 
                Encoding.UTF8);
        }

        public HttpRequest Content(
            string content,
            string mediaType,
            Encoding encoding)
        {
            _message.Content = new StringContent(
                content, 
                encoding, 
                mediaType);
            return this;
        }

        public HttpRequest Authenticator(
            IAuthenticator authenticator)
        {
            _authenticator = authenticator;

            return this;
        }

        public async Task<HttpResponse> ExecuteAsync()
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                AddDefaults();

                _message.RequestUri = _baseAddress.AddPathParameters(
                    _path,
                    _pathParameters);
                
                _authenticator?.Authenticate(this);

                if (_message.Content == null)
                {
                    _parameterHandler.AddParameters(
                        _message,
                        MediaTypeEnum.Form, 
                        _queryParameters);
                }
                else
                {
                    //TODO: allow content
                    throw new NotImplementedException();
                }

                var response = await client
                    .SendAsync(_message)
                    .ConfigureAwait(false);

                return new HttpResponse(
                    response.Content,
                    response.Headers,
                    response.StatusCode,
                    response.ReasonPhrase);
            }
        }

        private void AddDefaults()
        {
            if (_message.Headers.Accept.Count == 0)
            {
                Accepts(MediaTypeEnum.Json)
                    .Accepts(MediaTypeEnum.Xml)
                    .Accepts(MediaTypeEnum.TextJson)
                    .Accepts(MediaTypeEnum.TextXJson)
                    .Accepts(MediaTypeEnum.Javascript)
                    .Accepts(MediaTypeEnum.TextXml);
            }

            if (_message.Headers.AcceptEncoding.Count == 0)
            {
                AcceptsEncoding("gzip")
                    .AcceptsEncoding("deflate");
            }
        }
    }
}

