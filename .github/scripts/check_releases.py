#!/usr/bin/env python3
"""
Script to check for new releases in configured repositories.
"""
import os
import json
import re
import github
from github import Github

# Configuration files
MOD_LIST_FILE = "ModList.json"
RELEASE_VERSIONS_FILE = "release_versions.json"

def load_mod_list():
    """Load repository list from ModList.json"""
    if os.path.exists(MOD_LIST_FILE):
        with open(MOD_LIST_FILE, 'r') as f:
            return json.load(f)

def extract_repo_info(repo_url):
    """Extract owner and repository name from a GitHub URL"""
    # Handle formats like https://github.com/owner/repo or github.com/owner/repo
    pattern = r"(?:https?://)?(?:www\.)?github\.com[/:]([^/]+)/([^/]+)(?:\.git)?/?$"
    match = re.match(pattern, repo_url)
    if match:
        return f"{match.group(1)}/{match.group(2)}"
    return None

def get_latest_release(repo_name, g):
    """Get the latest release for a repository"""
    try:
        repo = g.get_repo(repo_name)
        latest_release = repo.get_latest_release()
        return latest_release.tag_name
    except github.GithubException as e:
        print(f"Error fetching release for {repo_name}: {e}")
        return None

def generate_versions_json(versions_data):
    """Generate JSON file with guid and version fields for all mods"""
    versions_json = []
    
    for item in versions_data:
        versions_json.append({
            "guid": item["guid"],
            "version": item["version"]
        })
    
    with open(RELEASE_VERSIONS_FILE, "w") as f:
        json.dump(versions_json, f, indent=2)

def main():
    github_token = os.environ.get("GITHUB_TOKEN")
    if not github_token:
        print("GITHUB_TOKEN not provided")
        return
    
    g = Github(github_token)
    mods = load_mod_list()
    all_versions = []
    has_updates = False
    
    for mod in mods:
        guid = mod.get("guid")
        repo_url = mod.get("repo")
        
        if not guid or not repo_url:
            print(f"Skipping invalid mod entry: {mod}")
            continue
        
        repo_name = extract_repo_info(repo_url)
        if not repo_name:
            print(f"Could not extract repository info from URL: {repo_url}")
            continue
        
        print(f"Checking releases for {repo_name} (GUID: {guid})")
        latest_version = get_latest_release(repo_name, g)
        
        version_entry = {
            "guid": guid,
            "version": latest_version if latest_version else "none"
        }
        
        all_versions.append(version_entry)
        
        if latest_version:
            print(f"Latest version for {repo_name} (GUID: {guid}): {latest_version}")
            has_updates = True
    
    # Generate JSON file with all versions
    generate_versions_json(all_versions)
    
    if has_updates:
        print(f"Updated versions for {len(all_versions)} mods")
    else:
        print("No versions found")

if __name__ == "__main__":
    main()
