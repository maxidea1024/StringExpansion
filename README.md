# StringExpansion
간단한  C#용 문자열 펼치기 관련한 간단한 조각 코드입니다.

`makefile`이나 `rpm spec` 파일을 사용하다보면, `${name}` 과 같이 환경변수나 약속한 값들로 치환하는 것을 볼수 있습니다. 이러한 동작을 수행하는 간단한 코드 조각입니다.

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

급하게 작성한거라 테스트 케이스 및 설명은 나중에 보충하도록 하겠습니다. ㅠㅠ
