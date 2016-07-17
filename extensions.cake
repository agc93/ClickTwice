public void AddIfNotExists(List<FilePath> list, FilePath file) {
    if (list.Any(f => f.FullPath.EndsWith(file.GetFilename().ToString()))) return;
    list.Add(file);
}

public void AddIfNotExists(List<FilePath> list, IEnumerable<FilePath> files) {
    foreach (var file in files) {
        AddIfNotExists(list, file);
    }
}
