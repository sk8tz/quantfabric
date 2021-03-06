﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using CodeGen.Class;
using CodeGen.Metadata;
using CodeGen.PropertyValues;
using Foundations.HttpClient.Enums;
using Material.Enums;
using Material.Metadata;
using Material.Infrastructure;
using Material.Infrastructure.Credentials;
using Newtonsoft.Json.Linq;

namespace CodeGen
{
    public class SwaggerToClass
    {
        private readonly JObject _swagger;

        public SwaggerToClass(string pathToSwaggerFile)
        {
            _swagger = JObject.Parse(File.ReadAllText(pathToSwaggerFile));
        }

        public ClassRepresentation GenerateServiceClass(string @namespace)
        {
            var @class = new ClassRepresentation(_swagger["info"]["title"].ToString(), @namespace)
            {
                Comments = _swagger["info"]["description"].ToString() + " " + _swagger["info"]["version"].ToString()
            };

            var securityDefinitions = _swagger["securityDefinitions"];

            foreach (var securityDefinition in securityDefinitions.Values())
            {
                if (securityDefinition["type"]?.ToString() == "oauth2")
                {
                    @class.BaseType = new BaseTypeRepresentation(typeof(OAuth2ResourceProvider));

                    if (@class.Properties.Count > 0)
                    {
                        //Here we are merging the implicit and code workflows
                        //TODO: should probably check the other parameters here and verify they match
                        var flows = @class.Properties.Single(p => p.Name == "Flows");
                        ((List<ResponseTypeEnum>)((ConcreteValueRepresentation)flows.PropertyValue).PropertyValue).Add(StringToEnum(securityDefinition["flow"].ToString()));

                        if (securityDefinition["tokenUrl"] != null)
                        {
                            @class.Properties.Add(new PropertyRepresentation(typeof(Uri), "TokenUrl")
                            {
                                IsOverride = true,
                                PropertyValue = new ConcreteValueRepresentation(new Uri(securityDefinition["tokenUrl"].ToString()))
                            });
                        }
                    }
                    else
                    {
                        @class.Properties.Add(new PropertyRepresentation(typeof(Uri), "AuthorizationUrl")
                        {
                            IsOverride = true,
                            PropertyValue = new ConcreteValueRepresentation(new Uri(securityDefinition["authorizationUrl"].ToString()))
                        });
                        if (securityDefinition["tokenUrl"] != null)
                        {
                            @class.Properties.Add(new PropertyRepresentation(typeof(Uri), "TokenUrl")
                            {
                                IsOverride = true,
                                PropertyValue = new ConcreteValueRepresentation(new Uri(securityDefinition["tokenUrl"].ToString()))
                            });
                        }
                        @class.Properties.Add(new PropertyRepresentation(typeof(List<string>), "AvailableScopes")
                        {
                            IsOverride = true,
                            PropertyValue = new ConcreteValueRepresentation(securityDefinition["scopes"]
                                .ToObject<JObject>()
                                .Properties()
                                .Select(p => p.Name)
                                .ToList())
                        });
                        @class.Properties.Add(new PropertyRepresentation(typeof(List<ResponseTypeEnum>), "Flows")
                        {
                            IsOverride = true,
                            PropertyValue = new ConcreteValueRepresentation(new List<ResponseTypeEnum> { StringToEnum(securityDefinition["flow"].ToString())})
                        });
                        if (securityDefinition["x-token-name"] != null)
                        {
                            @class.Properties.Add(new PropertyRepresentation(typeof(string), "TokenName")
                            {
                                IsOverride = true,
                                PropertyValue = new ConcreteValueRepresentation(securityDefinition["x-token-name"].ToString())
                            });
                        }
                        if (securityDefinition["x-scope-delimiter"] != null)
                        {
                            @class.Properties.Add(new PropertyRepresentation(typeof(char), "ScopeDelimiter")
                            {
                                IsOverride = true,
                                PropertyValue = new ConcreteValueRepresentation(securityDefinition["x-scope-delimiter"].ToString().ToCharArray()[0])
                            });
                        }

                        @class.Metadatas.Add(new ConcreteMetadataRepresentation(typeof(CredentialType))
                        {
                            ConstructorParameters = new List<object> { typeof(OAuth2Credentials) }
                        });

                    }
                }
                else if (securityDefinition["type"]?.ToString() == "oauth1")
                {
                    @class.BaseType = new BaseTypeRepresentation(typeof(OAuth1ResourceProvider));

                    @class.Properties.Add(new PropertyRepresentation(typeof(Uri), "RequestUrl")
                    {
                        IsOverride = true,
                        PropertyValue = new ConcreteValueRepresentation(new Uri(securityDefinition["requestUrl"].ToString()))
                    });
                    @class.Properties.Add(new PropertyRepresentation(typeof(Uri), "AuthorizationUrl")
                    {
                        IsOverride = true,
                        PropertyValue = new ConcreteValueRepresentation(new Uri(securityDefinition["authorizationUrl"].ToString()))
                    });
                    @class.Properties.Add(new PropertyRepresentation(typeof(Uri), "TokenUrl")
                    {
                        IsOverride = true,
                        PropertyValue = new ConcreteValueRepresentation(new Uri(securityDefinition["tokenUrl"].ToString()))
                    });
                    @class.Properties.Add(new PropertyRepresentation(typeof(OAuthParameterTypeEnum), "ParameterType")
                    {
                        IsOverride = true,
                        PropertyValue = new ConcreteValueRepresentation(StringToParameterTypeEnum(securityDefinition["x-parameter-type"].ToString()))
                    });
                    @class.Metadatas.Add(new ConcreteMetadataRepresentation(typeof(CredentialType))
                    {
                        ConstructorParameters = new List<object> { typeof(OAuth1Credentials) }
                    });
                }
                else
                {
                    throw new Exception();
                }
            }

            return @class;
        }

        public List<ClassRepresentation> GenerateRequestClasses(
            string @namespace,
            string serviceTypeName,
            string serviceTypeNamespace)
        {
            var classes = new List<ClassRepresentation>();

            var scheme = _swagger["schemes"][0].ToString();
            var domain = _swagger["host"].ToString();
            var pathRoot = _swagger["basePath"].ToString();

            //TODO: need to get the "produces" and "consumes" from here

            var paths = _swagger["paths"];

            foreach (JProperty path in paths)
            {
                var pathResidual = path.Name;

                foreach (JProperty request in path.Value)
                {
                    var details = request.Value.ToObject<JObject>();

                    var @class = new ClassRepresentation(
                        details["operationId"].ToString(),
                        @namespace)
                    {
                        Comments = details["summary"].ToString()
                    };

                    @class.Metadatas.Add(new AbstractMetadataRepresentation(typeof(ServiceType), serviceTypeNamespace, serviceTypeName));

                    @class.BaseType = new BaseTypeRepresentation(typeof(OAuthRequest));

                    @class.Properties.Add(new PropertyRepresentation(typeof(string), "Host")
                    {
                        IsOverride = true,
                        PropertyValue = new ConcreteValueRepresentation($"{scheme}://{domain}")
                    });
                    @class.Properties.Add(new PropertyRepresentation(typeof(string), "Path")
                    {
                        IsOverride = true,
                        PropertyValue = new ConcreteValueRepresentation((pathRoot + pathResidual).Replace("//", "/"))
                    });
                    @class.Properties.Add(new PropertyRepresentation(typeof(string), "HttpMethod")
                    {
                        IsOverride = true,
                        PropertyValue = new ConcreteValueRepresentation(request.Name.ToUpper())
                    });
                    if (details["x-request-filter-property"] != null)
                    {
                        @class.Properties.Add(new PropertyRepresentation(typeof(string), "RequestFilterKey")
                        {
                            IsOverride = true,
                            PropertyValue = new ConcreteValueRepresentation(details["x-request-filter-property"].ToString())
                        });
                    }
                    if (details["consumes"] != null)
                    {
                        var headers = details["consumes"].ToObject<JArray>().Select(t => t.ToString());
                        var header = string.Join(",", headers);

                        var currentHeaderProperty = @class.Properties.SingleOrDefault(p => p.Name == "Headers");

                        if (currentHeaderProperty == null)
                        {
                            @class.Properties.Add(
                                new PropertyRepresentation(typeof(Dictionary<HttpRequestHeader, string>), "Headers")
                                {
                                    IsOverride = true,
                                    PropertyValue = new ConcreteValueRepresentation(new Dictionary<HttpRequestHeader, string> {{HttpRequestHeader.Accept, header}})
                                });
                        }
                        else
                        {
                            //TODO: this will error out if there is already an accept header. should we be using something other than a dictionary,
                            //like NameValueCollection or HttpCollection???
                            ((Dictionary<HttpRequestHeader,string>)((ConcreteValueRepresentation)currentHeaderProperty.PropertyValue).PropertyValue).Add(HttpRequestHeader.Accept, header);
                        }
                    }
                    //TODO: this is pretty ridgid and makes assumptions about the structure that may not be true
                    if (details["security"]?.ToObject<JArray>()?.Count > 0 &&
                        details["security"]?.ToObject<JArray>()[0]?.ToObject<JObject>()?["oauth2"] != null)
                    {
                        var scopes =
                            details["security"]
                            .ToObject<JArray>()[0]
                            .ToObject<JObject>()["oauth2"]
                            .ToObject<JArray>()
                            .Select(s => s.ToString())
                            .ToList();

                        @class.Properties.Add(new PropertyRepresentation(typeof(List<string>), "RequiredScopes")
                        {
                            IsOverride = true,
                            PropertyValue = new ConcreteValueRepresentation(scopes)
                        });
                    }

                    string requestFilterProperty = null;

                    if (details["parameters"] != null)
                    {
                        foreach (JObject parameter in details["parameters"])
                        {
                            if (parameter["x-request-filter-property"] != null)
                            {
                                requestFilterProperty = parameter["x-request-filter-property"].ToString();
                            }

                            var name = CreateCSharpPropertyName(parameter["name"].ToString());
                            //TODO: here we have to determine if this will constitute a new enum
                            //and then figure out how to represent that new enum as part of this property

                            PropertyRepresentation property = null;

                            if (parameter["enum"] != null)
                            {
                                var enumName = @class.Name + name + "Enum";
                                var enumNamespace = "Foundations.Attributes";
                                var @enum = new EnumRepresentation(
                                    enumName, 
                                    enumNamespace);

                                foreach (var item in parameter["enum"])
                                {
                                    @enum.Values.Add(
                                        CreateCSharpPropertyName(item.ToString()), 
                                        item.ToString());
                                }

                                @class.Enums.Add(@enum);

                                ValueRepresentation propertyValue = null;
                                if (parameter["default"] != null)
                                {
                                    propertyValue = new NewEnumValueRepresentation(
                                        @enum.Name,
                                        CreateCSharpPropertyName(parameter["default"].ToString()));
                                }

                                property = new PropertyRepresentation(
                                    @enum.Name,
                                    @namespace,
                                    name)
                                {
                                    IsAutoProperty = true,
                                    PropertyValue = propertyValue,
                                    Comments = parameter["description"].ToString()
                                };
                            }
                            else
                            {
                                var type = GetTypeFromParameterType(
                                    parameter["type"]?.ToString(),
                                    parameter["format"]?.ToString(),
                                    parameter["required"]?.ToString());


                                ConcreteValueRepresentation propertyValue = null;

                                if (parameter["default"] != null)
                                {
                                    propertyValue = new ConcreteValueRepresentation(ConvertType(
                                        parameter["default"].ToString(),
                                        type));
                                }

                                property = new PropertyRepresentation(type, name)
                                {
                                    IsAutoProperty = true,
                                    PropertyValue = propertyValue,
                                    Comments = parameter["description"].ToString()
                                };
                            }

                            var nameMetadata = new ConcreteMetadataRepresentation(typeof(Name));
                            nameMetadata.ConstructorParameters = new List<object> { parameter["name"].ToString()};
                            property.Metadatas.Add(nameMetadata);


                            if (parameter["in"].ToString() == "query")
                            {
                                //querystringParameters.Add(parameter["name"].ToString(), name);
                                var propertyTypeMetadata = new ConcreteMetadataRepresentation(typeof(ParameterType));
                                propertyTypeMetadata.ConstructorParameters = new List<object> { RequestParameterTypeEnum.Query };
                                property.Metadatas.Add(propertyTypeMetadata);

                            }
                            else if (parameter["in"].ToString() == "path")
                            {
                                //urlsegmentParameters.Add(parameter["name"].ToString(), name);
                                var propertyTypeMetadata = new ConcreteMetadataRepresentation(typeof(ParameterType));
                                propertyTypeMetadata.ConstructorParameters = new List<object> { RequestParameterTypeEnum.Path };
                                property.Metadatas.Add(propertyTypeMetadata);
                            }
                            else if (parameter["in"].ToString() == "header")
                            {
                                var propertyTypeMetadata = new ConcreteMetadataRepresentation(typeof(ParameterType));
                                propertyTypeMetadata.ConstructorParameters = new List<object> { RequestParameterTypeEnum.Header };
                                property.Metadatas.Add(propertyTypeMetadata);
                            }
                            else
                            {
                                //cover "body" here
                                throw new NotImplementedException();
                            }

                            if (parameter["required"]?.ToString() == "true")
                            {
                                property.Metadatas.Add(new ConcreteMetadataRepresentation(typeof(Required)));
                            }

                            if (parameter["pattern"]?.ToString() != null)
                            {
                                var formatMetadata = new ConcreteMetadataRepresentation(typeof(Format));
                                formatMetadata.ConstructorParameters = new List<object> { parameter["pattern"]?.ToString()};
                                property.Metadatas.Add(formatMetadata);
                            }

                            @class.Properties.Add(property);
                        }
                    }


                    //handle the response
                    //if the response contains x-timeseries-information then append the ITimeseries interface
                    //if the response contains the x-response-filter property AND there is a parameter with the x-request-filter property, then implement the IFilterable (and implicitly the ITimeseries) interface

                    classes.Add(@class);
                }
            }

            return classes;
        }

        private object ConvertType(
            string item, 
            Type type)
        {
            if (type == typeof(string))
            {
                return item;
            }
            else if (type == typeof(int) || type == typeof(Nullable<int>))
            {
                return Convert.ToInt32(item);
            }
            else if (type == typeof(long) || type == typeof(Nullable<long>))
            {
                return Convert.ToInt64(item);
            }
            else if (type == typeof(DateTime) || type == typeof(Nullable<DateTime>))
            {
                DateTime result;

                if (DateTime.TryParse(item, out result))
                {
                    return result;
                }
                else
                {
                    throw new Exception("Couldn't parse datetime string");
                }
            }
            else if (type == typeof(DateTimeOffset) || type == typeof(Nullable<DateTimeOffset>))
            {
                DateTimeOffset result;

                if (DateTimeOffset.TryParse(item, out result))
                {
                    return result;
                }
                else
                {
                    throw new Exception("Couldn't parse datetime string");
                }
            }
            else
            {
                throw new Exception("Unknown object type");
            }
        }

        private Type GetTypeFromParameterType(
            string type, 
            string format,
            string required)
        {
            var isRequired = required == "true";

            if (type == "string")
            {
                if (format == null)
                {
                    return typeof(string);
                }
                if (format == "binary")
                {
                    return isRequired ? typeof(int) : typeof(int?);
                }
                if (format == "byte")
                {
                    return isRequired ? typeof(byte) : typeof(byte?);
                }
                if (format == "date" || format == "date-time")
                {
                    return isRequired ? typeof(DateTime) : typeof(DateTime?);
                }
                if (format == "date-time-offset")
                {
                    return isRequired ? typeof(DateTimeOffset) : typeof(DateTimeOffset?);
                }
            }
            else if (type == "number")
            {
                if (format == null || format == "double")
                {
                    return isRequired ? typeof(double) : typeof(double?);
                }
                if (format == "float")
                {
                    return isRequired ? typeof(float) : typeof(float?);
                }
            }
            else if (type == "integer")
            {
                if (format == null || format == "int32")
                {
                    return isRequired ? typeof(int) : typeof(int?);
                }
                if (format == "int64")
                {
                    return isRequired ? typeof(long) : typeof(long?);
                }
            }
            else if (type == "array")
            {
                return typeof(object[]);
            }
            else if (type == "boolean")
            {
                return isRequired ? typeof(bool) : typeof(bool?);
            }
            else if (type == "file")
            {
                throw new Exception();
            }

            throw new Exception();
        }

        private ResponseTypeEnum StringToEnum(string responseType)
        {
            if (responseType == "implicit")
            {
                return ResponseTypeEnum.Token;
            }
            else if (responseType == "accessCode")
            {
                return ResponseTypeEnum.Code;
            }
            else
            {
                throw new Exception($"ResponseType was {responseType}");
            }
        }

        private OAuthParameterTypeEnum StringToParameterTypeEnum(string parameterType)
        {
            if (parameterType == "querystring")
            {
                return OAuthParameterTypeEnum.Querystring;
            }
            else if (parameterType == "header")
            {
                return OAuthParameterTypeEnum.Header;
            }
            else if (parameterType == "body")
            {
                return OAuthParameterTypeEnum.Body;
            }
            else
            {
                throw new Exception();
            }
        }


        private string CreateCSharpPropertyName(string jsonName)
        {
            if (string.IsNullOrEmpty(jsonName))
            {
                return string.Empty;
            }

            var result = jsonName.Split(new[] { "_", "-" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
                .Aggregate(string.Empty, (s1, s2) => s1 + s2);

            return char.ToUpper(result[0]) + result.Substring(1);
        }
    }
}
