# StringExpansion
간단한  C#용 문자열 펼치기 관련한 간단한 조각 코드입니다.

`makefile`이나 `rpm spec` 파일을 사용하다보면, `${name}`, `%(name)` 과 같이 환경변수나 약속한 값들로 치환하는 것을 볼수 있습니다. 이러한 동작을 수행하는 간단한 코드 조각입니다.

```csharp
var options = new StringExpansionOptions
{
    VarProvider = new DummyVarProvider()
};

// 빈문자열 대입.
Test("Empty", "", options);

// var1, val1 값을 가져와 치환.
Test("Simple", "%var1=%val1", options);

// var4가 없으면 var3을 사용하고 그것도 없으면 var2를 사용.
Test("Alternatives", "%(var4:%(var3:%var2))", options);
```

`IVarProvider`를 잘 황용하면 다양한 기능을 손쉽게 확장할 수 있습니다. 예를들면, 환경 변수 또는 시스템 경로등을 제공할 수 있으므로, `.config` 파일등에 사용될 수 있습니다.

저는 현재 boilerplate 코드 생성 도구에 사용하고 있습니다.
제가 사용하고 있는 코드의 한 조각입니다.

#### 타입스크림트용 코드를 생성하는 곳에서 사용되고 있는 코드 샘플

```csharp
ts.Verbatim(@"

    // Indexing by '%prop_name'
    public get recordsBy%(pascal_name)(): Map<%(field_type), %(record_type)> { return this._recordsBy%pascal_name }
    private _recordsBy%pascal_name: Map<%field_type, %record_type> = new Map<%field_type, %record_type>()

    /** Gets the value associated with the specified key. throw Error if not found. */
    public getBy%pascal_name(key: %field_type): %record_type {
        const found = this._recordsBy%pascal_name.get(key)
        if (!found)
            throw new Error(`There is no record in table ""%table_name"" that corresponds to field ""%prop_name"" value ${key}`)

        return found
    }

    /** Gets the value associated with the specified key. */
    public tryGetBy%pascal_name(key: %field_type): %record_type | undefined {
        return this._recordsBy%pascal_name.get(key)
    }

    /** Determines whether the table contains the specified key. */
    public contains%pascal_name(key: %field_type): boolean {
        return !!this._recordsBy%pascal_name.has(key)
    }"
);
```
