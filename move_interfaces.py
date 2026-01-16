import os
import shutil

root = r"c:\Users\Kodake\Desktop\PosAPI"
infra_repos = os.path.join(root, "PosAPI.Infrastructure", "Repositories")
core_interfaces = os.path.join(root, "PosAPI.Core", "Interfaces")

def move_interfaces():
    for dirpath, dirnames, filenames in os.walk(infra_repos):
        for filename in filenames:
            if filename.startswith("I") and len(filename) > 1 and filename[1].isupper():
                rel_path = os.path.relpath(dirpath, infra_repos)
                dest_dir = os.path.join(core_interfaces, rel_path)
                if not os.path.exists(dest_dir):
                    os.makedirs(dest_dir)
                
                src = os.path.join(dirpath, filename)
                dest = os.path.join(dest_dir, filename)
                print(f"Moving {src} to {dest}")
                shutil.move(src, dest)

if __name__ == "__main__":
    move_interfaces()
