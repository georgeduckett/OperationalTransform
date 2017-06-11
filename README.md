A C# implementation of Operational Transforms
--

An in-development implementation of [Operational Transformation](https://en.wikipedia.org/wiki/Operational_transformation); a system for supporting the simultaneous editing of documents.

Currently consists of a library project; OperationalTransform containing all the logic, plus a test project that should ensure correctness (according to the principals of Operational Transforms).

The [wikipedia page](https://en.wikipedia.org/wiki/Operational_transformation) has a good overview, but the main principal is that any number of users could be making changes at any one time, so the order in which the changes are applied to a document must not matter; the system should work out the intent of the change at the origin site and translate it into something that can be applied locally.

e.g:  
With an initial document of `1234567890`  
User a inserts an "a" at index 5 to make their local state `12345a67890`  
User b inserts a "b" at index 8 to make their local state `12345678b90`  
User a receives b's change [insert b at index 8].

If this was just applied with no translation the result would be `12345a67b890`. B's change isn't applied as intended because we're trying to apply it to a's copy of the document which (because of a's change) isn't what b's change should be applied to. In other words, [insert b at index 8] indicates how to change the initial document, not a's copy.

What needs to happen when user a receives b's change is for user a to compare their document state to the document state b is intended to be applied to. Here "document state" can be thought of as the list of operations prior to the one being evaluated.

e.g:

User a receives b's change. User a finds a change that wasn't included in the document state that b's change was applied to (the [insert a at index 5] command), therefore we need to translate the b command by a's command before applying it. Because a's command is an insert, at an index before b's command we change b's command from [insert b at index 8] to [insert b at index 9], so the document state correctly becomes `12345a678b90`.

---

The solution to the above consists of a couple of parts:

- The classes that inherit from [`OperationBase`](https://github.com/georgeduckett/OperationalTransform/blob/master/OperationalTransform/Operations/OperationBase.cs) represent the different operations that can be applied to a document (insert, delete and the identity operation).
- The class [`AppliedOperation`](https://github.com/georgeduckett/OperationalTransform/blob/master/OperationalTransform/Operations/AppliedOperation.cs) represents applying an operation to a document; it consistes of the operation and unique id's of all operations applied thus far. This is what would be sent over the wire.
- The [`OperationTransformer`](https://github.com/georgeduckett/OperationalTransform/blob/master/OperationalTransform/Operations/OperationTransformer.cs) static class provides the `Transform` method that transforms an operation with respect to another (e.g. translates a received operation with respect to one locally applied).
- The [`DocumentState`](https://github.com/georgeduckett/OperationalTransform/blob/master/OperationalTransform/StateManagement/DocumentState.cs) class manages the document state itself. This includes assiging unique id's local for operations applied to it, keeping track of the applied operations and transforming incoming remote operations as appropriate.