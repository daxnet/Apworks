<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="37648ba3-512d-4438-b0c0-a03ba1b2c700" namespace="Apworks.Config" xmlSchemaNamespace="http://apworks.codeplex.com/Schemas/Configuration" assemblyName="Apworks" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
    <enumeratedType name="HandlerKind" namespace="Apworks.Config">
      <literals>
        <enumerationLiteral name="Command" />
        <enumerationLiteral name="Event" />
      </literals>
    </enumeratedType>
    <enumeratedType name="HandlerSourceType" namespace="Apworks.Config">
      <literals>
        <enumerationLiteral name="Type" />
        <enumerationLiteral name="Assembly" />
      </literals>
    </enumeratedType>
    <enumeratedType name="ExceptionHandlingBehavior" namespace="Apworks.Config">
      <literals>
        <enumerationLiteral name="Direct" />
        <enumerationLiteral name="Forward" />
      </literals>
    </enumeratedType>
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="ApworksConfigSection" documentation="Represents the configuration section for Apworks framework." codeGenOptions="XmlnsProperty" xmlSectionName="apworks">
      <elementProperties>
        <elementProperty name="Application" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="application" isReadOnly="false" documentation="The configuration for the Apworks application.">
          <type>
            <configurationElementMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/ApplicationElement" />
          </type>
        </elementProperty>
        <elementProperty name="ObjectContainer" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="objectContainer" isReadOnly="false" documentation="The configuration for the object container.">
          <type>
            <configurationElementMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/ObjectContainerElement" />
          </type>
        </elementProperty>
        <elementProperty name="Serializers" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="serializers" isReadOnly="false" documentation="The configuration for serializers.">
          <type>
            <configurationElementMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/SerializerElement" />
          </type>
        </elementProperty>
        <elementProperty name="Generators" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="generators" isReadOnly="false" documentation="The configuration for identity and sequential generators.">
          <type>
            <configurationElementMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/GeneratorElement" />
          </type>
        </elementProperty>
        <elementProperty name="Handlers" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="handlers" isReadOnly="false" documentation="The configuration for command or event handlers.">
          <type>
            <configurationElementCollectionMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/HandlerElementCollection" />
          </type>
        </elementProperty>
        <elementProperty name="Exceptions" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="exceptions" isReadOnly="false" documentation="The configuration for exception handling logic within Apworks framework.">
          <type>
            <configurationElementCollectionMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/ExceptionElementCollection" />
          </type>
        </elementProperty>
        <elementProperty name="Interception" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="interception" isReadOnly="false" documentation="The configuration for the interceptions.">
          <type>
            <configurationElementMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/InterceptionElement" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElement name="ApplicationElement" documentation="The application element">
      <attributeProperties>
        <attributeProperty name="Provider" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="provider" isReadOnly="false" documentation="The provider type of the application.">
          <type>
            <externalTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="ObjectContainerElement" documentation="The object container element.">
      <attributeProperties>
        <attributeProperty name="Provider" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="provider" isReadOnly="false" documentation="The provider type of the object container.">
          <type>
            <externalTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="InitFromConfigFile" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="initFromConfigFile" isReadOnly="false" documentation="The boolean value which indicates whether the object container configuration should be initialized from the app/web.config file." defaultValue="false">
          <type>
            <externalTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="SectionName" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="sectionName" isReadOnly="false" documentation="The name of the configuration section which would be used by the object container if it is designed to be initialized from the app/web.config file.">
          <type>
            <externalTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="EventSerializerElement" documentation="Represents the configuration for the event serializer.">
      <attributeProperties>
        <attributeProperty name="Provider" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="provider" isReadOnly="false" documentation="The provider type of the event serializer.">
          <type>
            <externalTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="SnapshotSerializerElement" documentation="Represents the configuration for the snapshot serializer.">
      <attributeProperties>
        <attributeProperty name="Provider" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="provider" isReadOnly="false" documentation="The provider type of the snapshot serializer.">
          <type>
            <externalTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="SerializerElement" documentation="Represents the configuration for either event or snapshot serializers.">
      <elementProperties>
        <elementProperty name="EventSerializer" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="eventSerializer" isReadOnly="false" documentation="The configuration for the event serializer.">
          <type>
            <configurationElementMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/EventSerializerElement" />
          </type>
        </elementProperty>
        <elementProperty name="SnapshotSerializer" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="snapshotSerializer" isReadOnly="false" documentation="The configuration for the snapshot serializer.">
          <type>
            <configurationElementMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/SnapshotSerializerElement" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElement name="SequenceGeneratorElement" documentation="Represents the configuration for the sequential generator.">
      <attributeProperties>
        <attributeProperty name="Provider" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="provider" isReadOnly="false" documentation="The type of sequence generator.">
          <type>
            <externalTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="IdentityGeneratorElement" documentation="Represents the configuration for the identity generator.">
      <attributeProperties>
        <attributeProperty name="Provider" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="provider" isReadOnly="false" documentation="The type of identity generator.">
          <type>
            <externalTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="GeneratorElement" documentation="The Generator element which contains the configuration for either sequential or identity generators.">
      <elementProperties>
        <elementProperty name="SequenceGenerator" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="sequenceGenerator" isReadOnly="false" documentation="The configuration for the sequence generator.">
          <type>
            <configurationElementMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/SequenceGeneratorElement" />
          </type>
        </elementProperty>
        <elementProperty name="IdentityGenerator" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="identityGenerator" isReadOnly="false" documentation="The configuration for the identity generator.">
          <type>
            <configurationElementMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/IdentityGeneratorElement" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElement name="HandlerElement" documentation="Represents the configuration for message handlers.">
      <attributeProperties>
        <attributeProperty name="Kind" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="kind" isReadOnly="false" documentation="The kind of the handler, can be either Command or Event.">
          <type>
            <enumeratedTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/HandlerKind" />
          </type>
        </attributeProperty>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false" documentation="The name of the handler.">
          <type>
            <externalTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="SourceType" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="sourceType" isReadOnly="false" documentation="The source type, can be either Assembly or Type.">
          <type>
            <enumeratedTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/HandlerSourceType" />
          </type>
        </attributeProperty>
        <attributeProperty name="Source" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="source" isReadOnly="false" documentation="The name of the source, which could be either the assembly name, when SourceType is Assembly, or the type name, when SourceType is Type.">
          <type>
            <externalTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="HandlerElementCollection" documentation="Represents the configuration collection which contains a set of configuration for message handlers." collectionType="BasicMap" xmlItemName="handler" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/HandlerElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="ExceptionHandlerElement" documentation="Represents the configuration for exception handlers.">
      <attributeProperties>
        <attributeProperty name="Type" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="type" isReadOnly="false" documentation="The type of the exception handler.">
          <type>
            <externalTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="ExceptionHandlerElementCollection" documentation="Represents the configuration collection which contains a set of configuration for the exception handlers." collectionType="BasicMap" xmlItemName="handler" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/ExceptionHandlerElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="ExceptionElement" documentation="Represents the configuration for exception handling.">
      <attributeProperties>
        <attributeProperty name="Type" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="type" isReadOnly="false" documentation="The type of the exception.">
          <type>
            <externalTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Behavior" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="behavior" isReadOnly="false" documentation="The behavior of the exception handling.">
          <type>
            <enumeratedTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/ExceptionHandlingBehavior" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="Handlers" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="handlers" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/ExceptionHandlerElementCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElementCollection name="ExceptionElementCollection" documentation="Represents the configuration collection which contains a set of configuration for exception handling." collectionType="BasicMap" xmlItemName="exception" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/ExceptionElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="InterceptorRefElement" documentation="Represents the configuration for interceptor ref.">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="InterceptorElement">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Type" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="type" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="InterceptorElementCollection" xmlItemName="interceptor" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/InterceptorElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="InterceptionElement" documentation="Represents the configuration for the interception.">
      <elementProperties>
        <elementProperty name="Interceptors" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="interceptors" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/InterceptorElementCollection" />
          </type>
        </elementProperty>
        <elementProperty name="Contracts" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="contracts" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/InterceptContractElementCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElementCollection name="InterceptorRefElementCollection" documentation="Represents the configuration for interceptor refs." xmlItemName="interceptorRef" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/InterceptorRefElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="InterceptMethodElement" documentation="Represents the configuration for intercept method.">
      <attributeProperties>
        <attributeProperty name="Signature" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="signature" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="InterceptorRefs" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="interceptorRefs" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/InterceptorRefElementCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElementCollection name="InterceptMethodElementCollection" xmlItemName="method" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/InterceptMethodElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="InterceptContractElement" documentation="Represents the configuration for intercept contract.">
      <attributeProperties>
        <attributeProperty name="Type" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="type" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="Methods" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="methods" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/InterceptMethodElementCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElementCollection name="InterceptContractElementCollection" documentation="Represents the configuration for intercept contracts." xmlItemName="contract" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/37648ba3-512d-4438-b0c0-a03ba1b2c700/InterceptContractElement" />
      </itemType>
    </configurationElementCollection>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>